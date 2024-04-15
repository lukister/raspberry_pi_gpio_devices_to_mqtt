namespace RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;

internal class TestTemperatureAndHumiditySensor : ISensor
{
    public Capability[] GetCapabilities()
    {
        var tempSensor = new Capability("temperature", "Temperature", "temperature",
            "{{ value_json.temperature }}", "°C");

        var humidytySenspr = new Capability("humidity", "Humidity", "humidity",
            "{{ value_json.humidity }}", "%");

        return [tempSensor, humidytySenspr];
    }

    public object ReadState()
    {
        var random = new Random();

        var telemetry = new TelemetryDataDto()
        {
            Temperature = random.Next(150, 220) / 10d,
            Humidity = random.Next(30, 60),
        };

        return telemetry;
    }

    private class TelemetryDataDto
    {
        public double Temperature { get; set; }

        public double Humidity { get; set; }
    }
}
