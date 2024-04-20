using System.Text.Json.Serialization;

namespace RaspberryPoGpioToMqtt.Devices.DeviceRepository;

internal class CapabilityEntity
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [JsonExtensionData]
    public Dictionary<string, object> AdditionalConfiguration { get; set; } = [];

    public T GetValue<T>(string name)
    {
        if (!AdditionalConfiguration.TryGetValue(name, out var value))
            throw new Exception();

        if (value is T)
            return (T)value;

        return (T)Convert.ChangeType(value, typeof(T));
    }
}