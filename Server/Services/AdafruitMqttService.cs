using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using Server;
using Server.Models;
using Server.Services;


public class AdafruitMqttService : IHostedService
{
    public event Action<string> AnnounceMessageReceived;
    public event Action<string> SensorMessageReceived;

    private readonly Settings _settings;
    private readonly HelperService _helperService;
    private IMqttClient _mqttClient;

    public AdafruitMqttService(Settings settings, HelperService helperService)
    {
        _settings = settings;
        _helperService = helperService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
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
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.DisconnectAsync();
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
        if (args.ApplicationMessage.Topic.Contains(_settings.AdafruitAnnounceFeedName)) AnnounceMessageReceived?.Invoke(decodedMessage);
        else if (args.ApplicationMessage.Topic.Contains(_settings.AdafruitSensorFeedName)) SensorMessageReceived?.Invoke(decodedMessage);

        Console.WriteLine("Message received");
        return Task.CompletedTask;
    }
}