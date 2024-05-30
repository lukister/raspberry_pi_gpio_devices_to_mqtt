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
    private readonly ILogger<MqttClient> _logger;
    private IMqttClient _mqttClient;

    public MqttClient(IOptions<MqttClientOptions> configuration, ILogger<MqttClient> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;
        _mqttClient = CreateMqttClient();
    }

    private IMqttClient CreateMqttClient()
    {
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += MessageRecived;
        mqttClient.DisconnectedAsync += async x =>
        {
            _logger.LogInformation("Client Disconected");
            await Task.Delay(2500);
            try
            {
                _logger.LogInformation("Try reconnect MQTT client");
                _mqttClient = CreateMqttClient();
                await Initialize();
                _logger.LogInformation("MQTT client Reconnected");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "MQTT client recconetion failed. Try to recreate client.");
            }
        };
        _logger.LogInformation("MQTT client created");
        return mqttClient;
    }

    public async Task<bool> KeepAlive()
    {
        if (_mqttClient.IsConnected)
        {
            try
            {
                await _mqttClient.PingAsync();
                return true;
            }
            catch { }
        }

        var oldClient = _mqttClient;

        try
        {
            var mqttClient = CreateMqttClient();
            await Connect(mqttClient, _configuration, _subscribedTopicActions.Keys);
            await mqttClient.PingAsync();
            _mqttClient = mqttClient;
            await oldClient.DisconnectAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to connect to mqtt");
            return false;
        }
    }

    public async Task Initialize()
    {
        await Connect(_mqttClient, _configuration, _subscribedTopicActions.Keys);
    }

    private static async Task Connect(IMqttClient mqttClient, MqttClientOptions configuration, IEnumerable<string> topics)
    {
        var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(configuration.Server)
            .WithCleanSession(true);

        if (!string.IsNullOrWhiteSpace(configuration.User)
            && !string.IsNullOrWhiteSpace(configuration.Password))
        {
            mqttClientOptionsBuilder.WithCredentials(configuration.User, configuration.Password);
        }
        var mqttClientOptions = mqttClientOptionsBuilder.Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        foreach (var topic in topics)
        {
            await mqttClient.SubscribeAsync(topic);
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
        try
        {
            if (!_subscribedTopicActions.TryGetValue(args.ApplicationMessage.Topic, out var action))
                return;

            await action(args.ApplicationMessage.ConvertPayloadToString());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to handle message from MQTT client. Topic: {Topic}", args.ApplicationMessage.Topic);
        }
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