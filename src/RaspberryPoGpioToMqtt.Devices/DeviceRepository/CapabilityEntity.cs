using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaspberryPoGpioToMqtt.Devices.DeviceRepository;

internal class CapabilityEntity
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalConfiguration { get; set; } = [];

    public T GetValue<T>(string name)
    {
        if (!AdditionalConfiguration.TryGetValue(name, out var value))
            throw new Exception($"{name} is missing in configuration");
        return value.Deserialize<T>() 
            ?? throw new Exception($"Unable to get value: {name} from {value}");
    }

    public T? GetOptionalValue<T>(string name)
    {
        if (!AdditionalConfiguration.TryGetValue(name, out var value))
            return default;
        return value.Deserialize<T>();
    }
}