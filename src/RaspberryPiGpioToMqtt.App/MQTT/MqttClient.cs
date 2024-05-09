using MQTTnet.Client;
using MQTTnet;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using RaspberryPoGpioToMqtt.Devices;

namespace RaspberryPiGpioToMqtt.App.MQTT;

public class MqttClientOptions
{
    public const string SectionName = "Mqtt";
    [Required]
    public string Server { get; set; } = string.Empty;
    [Required]
    public string User { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed class MqttClient : ICommunication, IAsyncDisposable
{
    private readonly MqttClientOptions _configuration;
    private IMqttClient _mqttClient;

    public MqttClient(IOptions<MqttClientOptions> configuration)
    {
        _configuration = configuration.Value;
        _mqttClient = CreateMqttClient();
    }

    private IMqttClient CreateMqttClient()
    {
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += MessageRecived;
        mqttClient.DisconnectedAsync += async x =>
        {
            await Task.Delay(2500);
            try
            {
                await mqttClient.ReconnectAsync();
            }
            catch
            {
                _mqttClient = CreateMqttClient();
                await Initialize();
            }
        };
        return mqttClient;
    }

    public async Task Initialize()
    {
        var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(_configuration.Server)
            .WithCleanSession(false);

        if (!string.IsNullOrWhiteSpace(_configuration.User)
            && !string.IsNullOrWhiteSpace(_configuration.Password))
        {
            mqttClientOptionsBuilder.WithCredentials(_configuration.User, _configuration.Password);
        }
        var mqttClientOptions = mqttClientOptionsBuilder.Build();

        await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        foreach (var topic in _subscribedTopicActions.Keys)
        {
            await _mqttClient.SubscribeAsync(topic);
        }
    }

    private async Task SendData(string topic, string payload)
    {
        var message = new MqttApplicationMessageBuilder()
         .WithTopic(topic)
         .WithPayload(payload)
         .Build();
        await _mqttClient.PublishAsync(message);
    }


    private Dictionary<string, Func<string, Task>> _subscribedTopicActions = [];

    private async Task MessageRecived(MqttApplicationMessageReceivedEventArgs args)
    {
        if (!_subscribedTopicActions.TryGetValue(args.ApplicationMessage.Topic, out var action))
            return;

        await action(args.ApplicationMessage.ConvertPayloadToString());
    }

    public async Task SubscribeFor(string topic, Func<string, Task> onMessageRecived)
    {
        _subscribedTopicActions.Add(topic, message => onMessageRecived(message));
        var subscription = await _mqttClient.SubscribeAsync(topic);
    }

    public async ValueTask DisposeAsync()
    {
        var disconectOptions = new MqttClientDisconnectOptionsBuilder()
            .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
            .Build();
        await _mqttClient.DisconnectAsync(disconectOptions);
        _mqttClient.Dispose();
    }

    public async Task Send(string topic, string message)
    {
        await SendData(topic, message);
    }
}