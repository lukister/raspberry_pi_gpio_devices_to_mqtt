using RaspberryPiGpioToMqtt.App;
using System.Globalization;

public class SensorHostedService(MqttClient client) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            var random = new Random();
            var temperature = (random.Next(150, 220) / 10d).ToString(CultureInfo.InvariantCulture);
            await client.SendData("greenhouse/in/temperature", temperature);
            var humidity = random.Next(30, 60).ToString();
            await client.SendData("greenhouse/in/humidity", humidity);
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}
