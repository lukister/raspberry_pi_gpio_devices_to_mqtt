using Microsoft.Extensions.Configuration;
using MQTTnet.Client;
using MQTTnet;

public sealed class MqttClient : IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private IMqttClient _mqttClient;

    public MqttClient(IConfiguration configuration)
    {
        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();
        _configuration = configuration;
    }

    private async Task ConnectIfDisconected()
    {
        if (_mqttClient == null || _mqttClient.IsConnected)
            return;
        var mqttClientOptions = new MqttClientOptionsBuilder()
        .WithTcpServer(_configuration["Server"]!)
        .WithCredentials(_configuration["User"]!, _configuration["Password"]!)
        .Build();

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