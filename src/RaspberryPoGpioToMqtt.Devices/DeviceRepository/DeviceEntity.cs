namespace RaspberryPoGpioToMqtt.Devices.DeviceRepository;

internal class DeviceEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CapabilityEntity[] Sensors { get; set; } = [];
    public CapabilityEntity[] Switches { get; set; } = []; 
    public CapabilityEntity[] Buttons { get; set; } = []; 
}
