﻿using MQTTnet;
using MQTTnet.Client;
using Server;
using Server.Models;
using Server.Services;


public class AdafruitMqttService : BackgroundService
{
    public event Action<string> AnnounceMessageReceived;
    public event Action<string> SensorMessageReceived;

    private readonly Settings _settings;
    private readonly HelperService _helperService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private IMqttClient _mqttClient;

    private List<AccumulatedPlantDataLog> _accumulatedPlantDataLogs = new();
    private string _accumulatorStartTimeString = DateTime.Now.ToString();

    public AdafruitMqttService(Settings settings, HelperService helperService, IServiceScopeFactory serviceScopeFactory)
    {
        _settings = settings;
        _helperService = helperService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async override Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("io.adafruit.com", 8883).WithCredentials(_settings.AdafruitUsername, _settings.AdafruitKey).WithTls().Build();
        _mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedHandler;
        await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic(_helperService.AnnounceTopicPath); })
            .WithTopicFilter(f => { f.WithTopic(_helperService.SensorTopicPath); })
            .WithTopicFilter(f => { f.WithTopic("Leo2308/feeds/dadn.test2"); })
            .Build();

        await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        Console.WriteLine("AdafruitMqttService initialized successfully.");

        SensorMessageReceived += SensorMessageReceivedHandler;
    }

    public async override Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        await _mqttClient.DisconnectAsync();
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (_accumulatedPlantDataLogs.Count == 0)
            {
                Console.WriteLine("WARNING: The server did not accumulate any log in the last timespan (" + _accumulatorStartTimeString + " to " + DateTime.Now + ").");
                continue;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

                foreach (var accumulatedPlantDataLog in _accumulatedPlantDataLogs)
                {
                    var ownerPlant = dbContext.PlantInformations.FirstOrDefault(info => info.Id == accumulatedPlantDataLog.PlantId);
                    if (ownerPlant == null) continue;

                    dbContext.PlantDataLogs.Add(new PlantDataLog()
                    {
                        Timestamp = DateTime.Now,
                        LightValue = accumulatedPlantDataLog.AveragedLightValue,
                        TemperatureValue = accumulatedPlantDataLog.AveragedTemperatureValue,
                        MoistureValue = accumulatedPlantDataLog.AveragedMoistureValue,
                        Owner = ownerPlant,
                    });
                }

                await dbContext.SaveChangesAsync(stoppingToken);

                _accumulatedPlantDataLogs = new();
                _accumulatorStartTimeString = DateTime.Now.ToString();
            }
        }
    }

    public async void PublishMessage(string topic, string content)
    {
        var applicationMessage = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(content).Build();
        await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
    }

    private Task ApplicationMessageReceivedHandler(MqttApplicationMessageReceivedEventArgs args)
    {
        var decodedMessage = _helperService.DecodeMqttPayload(args.ApplicationMessage.Payload);
        Console.WriteLine("From topic: " + args.ApplicationMessage.Topic);
        SensorMessageReceived?.Invoke(decodedMessage);

        Console.WriteLine("Message received");
        return Task.CompletedTask;
    }

    private void SensorMessageReceivedHandler(string content)
    {
        Console.WriteLine(content);
        AccumulateNewValues(content);
        Console.WriteLine("Accumulating");
    }

    private void AccumulateNewValues(string content)
    {
        var values = content[2..].Split(';').Select(float.Parse).ToList();

        if (values.Count > _accumulatedPlantDataLogs.Count)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

                for (int i = _accumulatedPlantDataLogs.Count; i < values.Count; i++)
                {
                    _accumulatedPlantDataLogs.Add(new AccumulatedPlantDataLog()
                    {
                        PlantId = dbContext.PlantInformations.ToList()[i].Id,
                    });
                }
            }
        }

        for (var i = 0; i < values.Count; i++)
        {
            switch (content[0])
            {
                case 'L':
                    _accumulatedPlantDataLogs[i].AccumulatedLightValue += values[i];
                    _accumulatedPlantDataLogs[i].LightValueCount++;
                    break;
                case 'T':
                    _accumulatedPlantDataLogs[i].AccumulatedTemperatureValue += values[i];
                    _accumulatedPlantDataLogs[i].TemperatureValueCount++;
                    break;
                case 'M':
                    _accumulatedPlantDataLogs[i].AccumulatedMoistureValue += values[i];
                    _accumulatedPlantDataLogs[i].MoistureValueCount++;
                    break;
            }
        }
    }
}