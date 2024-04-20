namespace RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;

internal class TestTemperatureAndHumiditySensor : ISensor
{
    public SensorCapability[] GetCapabilities()
    {
        var tempSensor = new SensorCapability("temperature", "Temperature", "temperature",
            "{{ value_json.temperature }}", "°C");

        var humidytySenspr = new SensorCapability("humidity", "Humidity", "humidity",
            "{{ value_json.humidity }}", "%");

        return [tempSensor, humidytySenspr];
    }

    public Task<object> ReadState()
    {
        var random = new Random();

        var telemetry = new TelemetryDataDto()
        {
            Temperature = random.Next(150, 220) / 10d,
            Humidity = random.Next(30, 60),
        };

        return Task.FromResult((object)telemetry);
    }

    private class TelemetryDataDto
    {
        public double Temperature { get; set; }

        public double Humidity { get; set; }
    }
}
