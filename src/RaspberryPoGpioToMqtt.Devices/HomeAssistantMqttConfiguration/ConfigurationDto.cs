namespace RaspberryPiGpioToMqtt.Devices.HomeAssistantMqttConfiguration;

internal class ConfigurationDto(string uniqueId)
{
    public string? Name { get; set; }
    public string? StateTopic { get; set; }
    public string? DeviceClass { get; set; }
    public string? CommandTopic { get; set; }
    public string? ValueTemplate { get; set; }
    public string UniqueId { get; set; } = uniqueId;
    public string? UnitOfMeasurement { get; set; }
    public DeviceDto? Device { get; set; }
    public List<AvailabilityDto> Availability { get; set; } = null!;
}
