using MQTTnet.Client;
using MQTTnet;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RaspberryPoGpioToMqtt.Devices;

namespace RaspberryPiGpioToMqtt.App.MQTT;

public class MqttClientOptions
{
    public const string SectionName = "Mqtt";
    [Required]
    public string Server { get; set; } = string.Empty;
    public string? User { get; set; }
    public string? Password { get; set; }
}

public sealed class MqttClient : ICommunication, IAsyncDisposable
{
    private readonly MqttClientOptions _configuration;
    private readonly IMqttClient _mqttClient;

    private JsonSerializerOptions _serializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public MqttClient(IOptions<MqttClientOptions> configuration)
    {
        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();
        _configuration = configuration.Value;
        _mqttClient.ApplicationMessageReceivedAsync += MessageRecived;
    }

    private async Task ConnectIfDisconected()
    {
        if (_mqttClient == null || _mqttClient.IsConnected)
            return;
        var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(_configuration.Server);

        if (!string.IsNullOrWhiteSpace(_configuration.User)
            && !string.IsNullOrWhiteSpace(_configuration.Password))
        {
            mqttClientOptionsBuilder.WithCredentials(_configuration.User, _configuration.Password);
        }
        var mqttClientOptions = mqttClientOptionsBuilder.Build();

        await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
    }

    private async Task SendData(string topic, string payload)
    {
        await ConnectIfDisconected();
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

    public async Task SubscribeFor<T>(string topic, Func<T, Task> onMessageRecived)
    {
        _subscribedTopicActions.Add(topic, message => onMessageRecived(
            JsonSerializer.Deserialize<T>(message) ?? throw new Exception("Unable to deserialize message")));
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

    public async Task Send<T>(string topic, T message)
    {
        var payload = JsonSerializer.Serialize(message, _serializerOptions);
        await SendData(topic, payload);
    }

    public async Task Send(string topic, string message)
    {
        await SendData(topic, message);
    }
}