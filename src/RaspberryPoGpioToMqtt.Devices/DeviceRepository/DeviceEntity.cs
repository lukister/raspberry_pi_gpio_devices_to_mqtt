namespace RaspberryPoGpioToMqtt.Devices.DeviceRepository;

internal class DeviceEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SensorEntity[] Sensors { get; set; } = [];
}
