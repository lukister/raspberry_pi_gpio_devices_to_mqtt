namespace RaspberryPiGpioToMqtt.Devices.HomeAssistantMqttConfiguration;

internal class DeviceDto
{
    public string? Name { get; set; }
    public string[] Identifiers { get; set; } = [];
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SuggestedArea { get; set; }
}