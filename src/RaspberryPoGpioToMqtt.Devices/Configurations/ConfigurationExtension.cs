
namespace RaspberryPoGpioToMqtt.Devices.Configurations;

internal static class ConfigurationExtension
{
    private const string _topicSeparator = "/";
    private const string _idSeparator = "_";

    public static string GetStateTopic(this CapabilityConfiguration conf)
        => string.Join(_topicSeparator, conf.DeviceId, conf.SensorId, "state");

    public static string GetCommandTopic(this CapabilityConfiguration conf)
        => string.Join(_topicSeparator, conf.DeviceId, conf.SensorId, "command");

    public static string GetAvalibilityTopic(this CapabilityConfiguration conf)
        => string.Join(_topicSeparator, conf.DeviceId, "avalibility");

    public static string GetAvalibilityTopic(this AvalibilityConfiguration conf)
        => string.Join(_topicSeparator, conf.DeviceId, "avalibility");

    public static string GetSensorDiscoveryTopic(string id)
        => GetDiscoveryTopic(id, "sensor");
    public static string GetSwitchDiscoveryTopic(string id)
        => GetDiscoveryTopic(id, "switch");
    internal static string GetButtonDiscoveryTopic(string id)
        => GetDiscoveryTopic(id, "button");

    private static string GetDiscoveryTopic(string id, string type) => string.Join(_topicSeparator,
        "homeassistant", type, id, "config");

    public static string GetFullId(this CapabilityConfiguration conf)
        => string.Join(_idSeparator, conf.DeviceId, conf.SensorId);

    public static string GetCapabilityId(this CapabilityConfiguration conf, string capabulityId)
        => string.Join(_idSeparator, conf.DeviceId, conf.SensorId, capabulityId);

    
}
