using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using Server.Models;

public class AdafruitMQTTService : IHostedService
{
    private readonly Settings _settings;
    private IMqttClient _mqttClient;

    public AdafruitMQTTService(Settings settings)
    {
        _settings = settings;
    }

    public static void PublishMessage(string topic, string content)
    {
    }

    private static Task MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs args)
    {
        Console.WriteLine("Message received");
        Console.WriteLine(args.DumpToConsole());
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var mqttFactory = new MqttFactory();

        _mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("io.adafruit.com", 8883).WithCredentials(_settings.AdafruitUsername, _settings.AdafruitKey).WithTls().Build();
        _mqttClient.ApplicationMessageReceivedAsync += MessageReceivedHandler;

        await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var applicationMessage = new MqttApplicationMessageBuilder().WithTopic("Leo2308/feeds/dadn.test2").WithPayload("19.5").Build();
        await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(f => { f.WithTopic("Leo2308/feeds/dadn.test2"); }).Build();
        await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.DisconnectAsync();
    }
}

internal static class ObjectExtensions
{
    public static TObject DumpToConsole<TObject>(this TObject @object)
    {
        var output = "NULL";
        if (@object != null)
        {
            output = JsonSerializer.Serialize(@object,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }

        Console.WriteLine($"[{@object?.GetType().Name}]:\r\n{output}");
        return @object;
    }
}