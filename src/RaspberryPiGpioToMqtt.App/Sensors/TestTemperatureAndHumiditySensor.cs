using System.Text.Json.Serialization;
using RaspberryPiGpioToMqtt.App.DTOs;

namespace RaspberryPiGpioToMqtt.App.Sensors;

public class TestTemperatureAndHumiditySensor : ISensor
{
    public string Id { get; }
    public string Name { get; }

    private string _telemetryTopic;

    public TestTemperatureAndHumiditySensor(string id, string name)
    {
        Id = id;
        Name = name;
        _telemetryTopic = $"{Id}/sensor";
    }

    public SensorDto[] GetConfigurations()
    {
        var discoveryDevice = new DeviceDto()
        {
            Identifiers = [Id],
            Manufacturer = "Fake",
            Model = "Device",
            Name = Name,
        };

        var tempSensor = new SensorDto()
        {
            Name = "Temperature",
            DeviceClass = "temperature",
            StateTopic = _telemetryTopic,
            ValueTemplate = "{{ value_json.temperature }}",
            UnitOfMeasurement = "°C",
            UniqueId = $"{discoveryDevice.Identifiers[0]}_temperature",
            Device = discoveryDevice,
        };

        var humidytySenspr = new SensorDto()
        {
            Name = "Humidity",
            DeviceClass = "humidity",
            StateTopic = _telemetryTopic,
            ValueTemplate = "{{ value_json.humidity }}",
            UnitOfMeasurement = "%",
            UniqueId = $"{discoveryDevice.Identifiers[0]}_humidity",
            Device = discoveryDevice,
        };

        return [tempSensor, humidytySenspr];
    }

    


    public TelemetryData GetTelemetry()
    {
        var random = new Random();

        var telemetry = new TelemetryDataDto()
        {
            Temperature = random.Next(150, 220) / 10d,
            Humidity = random.Next(30, 60),
        };

        return new TelemetryData(_telemetryTopic, telemetry);
    }

    private class TelemetryDataDto
    {
        public double Temperature { get; set; }

        public double Humidity { get; set; }
    }
}
