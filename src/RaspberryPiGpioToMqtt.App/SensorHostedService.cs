using RaspberryPiGpioToMqtt.App.MQTT;
using RaspberryPiGpioToMqtt.App.Sensors;

public class SensorHostedService(MqttClient client) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sensors = new List<ISensor>() 
        {
            new TestTemperatureAndHumiditySensor("fake_device_1", "Fake device 1")
        };
        await ConfigureSensors(client, sensors);

        while (!stoppingToken.IsCancellationRequested)
        {
            await SendSensorTelemetry(client, sensors);
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private static async Task SendSensorTelemetry(MqttClient client, List<ISensor> sensors)
    {
        foreach (var sensor in sensors)
        {
            var telemetry = sensor.GetTelemetry();
            await client.SendData(telemetry.Topic, telemetry.Data);
        }
    }

    private static async Task ConfigureSensors(MqttClient client, List<ISensor> devices)
    {
        foreach (var device in devices)
        {
            var configMessages = device.GetConfigurations();
            foreach (var message in configMessages)
            {
                var configurationTopic = $"homeassistant/sensor/{message.UniqueId}/config";
                await client.SendData(configurationTopic, message);
            }
        }
    }
}
