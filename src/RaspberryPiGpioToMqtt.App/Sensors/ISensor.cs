using RaspberryPiGpioToMqtt.App.DTOs;

namespace RaspberryPiGpioToMqtt.App.Sensors;

public interface ISensor
{
    string Id { get; }
    string Name { get; }
    SensorDto[] GetConfigurations();
    TelemetryData GetTelemetry();
}

public record TelemetryData(string Topic, object Data);