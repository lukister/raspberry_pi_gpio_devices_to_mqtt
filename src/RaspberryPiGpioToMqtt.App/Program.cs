using Microsoft.Extensions.Configuration;
using System.Globalization;


var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var client = new MqttClient(configuration.GetSection("Mqtt"));

try
{
    var random = new Random();
    var temperature = (random.Next(150, 220) / 10d).ToString(CultureInfo.InvariantCulture);
    await client.SendData("greenhouse/in/temperature", temperature);
    var humidity = random.Next(30, 60).ToString();
    await client.SendData("greenhouse/in/humidity", humidity);
}
finally
{

    await client.DisposeAsync();
}
