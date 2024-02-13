using MQTTnet.Client;
using MQTTnet;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace RaspberryPiGpioToMqtt.App;

public class MqttClientOptions
{
    public const string SectionName = "Mqtt";
    [Required]
    public string Server { get; set; } = string.Empty;
    public string? User { get; set; }
    public string? Password { get; set; }
}

public sealed class MqttClient : IAsyncDisposable
{
    private readonly MqttClientOptions _configuration;
    private readonly IMqttClient _mqttClient;

    public MqttClient(IOptions<MqttClientOptions> configuration)
    {
        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();
        _configuration = configuration.Value;
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

    public async Task SendData(string topic, string payload)
    {
        await ConnectIfDisconected();
        var message = new MqttApplicationMessageBuilder()
         .WithTopic(topic)
         .WithPayload(payload)
         .Build();
        await _mqttClient.PublishAsync(message);
    }

    public async ValueTask DisposeAsync()
    {
        var disconectOptions = new MqttClientDisconnectOptionsBuilder()
            .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
            .Build();
        await _mqttClient.DisconnectAsync(disconectOptions);
        _mqttClient.Dispose();
    }
}