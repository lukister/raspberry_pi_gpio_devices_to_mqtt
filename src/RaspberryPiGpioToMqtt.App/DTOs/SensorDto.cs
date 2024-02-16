namespace RaspberryPiGpioToMqtt.App.DTOs;

public class SensorDto
{
    public string? Name { get; set; }
    public string? StateTopic { get; set; }
    public string? DeviceClass { get; set; }
    public string? ValueTemplate { get; set; }
    public string? UniqueId { get; set; }
    public string? UnitOfMeasurement { get; set; }
    public DeviceDto? Device { get; set; }
}
