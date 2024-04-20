using System.Text.Json.Serialization;

namespace RaspberryPoGpioToMqtt.Devices.DeviceRepository;

internal class CapabilityEntity
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [JsonExtensionData]
    public Dictionary<string, object> AdditionalConfiguration { get; set; } = [];
}