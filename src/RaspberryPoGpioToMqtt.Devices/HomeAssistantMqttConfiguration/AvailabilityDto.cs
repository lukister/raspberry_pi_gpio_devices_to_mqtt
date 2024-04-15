namespace RaspberryPiGpioToMqtt.Devices.HomeAssistantMqttConfiguration;

internal class AvailabilityDto(string topic)
{
    public string Topic { get; set; } = topic;
}