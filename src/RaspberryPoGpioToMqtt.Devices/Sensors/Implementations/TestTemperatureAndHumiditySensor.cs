namespace RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;

internal class TestTemperatureAndHumiditySensor : BaseTemperatureAndHumiditySensor, ISensor
{
    public override Task<object> ReadState()
    {
        var random = new Random();

        var telemetry = new TemperatureAndHumidityTelemetryDto()
        {
            Temperature = random.Next(150, 220) / 10d,
            Humidity = random.Next(30, 60),
        };

        return Task.FromResult((object)telemetry);
    }
}
