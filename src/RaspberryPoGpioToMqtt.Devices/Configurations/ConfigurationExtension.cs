namespace RaspberryPoGpioToMqtt.Devices.Configurations;

internal static class ConfigurationExtension
{
    private const string _topicSeparator = "/";
    private const string _idSeparator = "_";

    public static string GetStateTopic(this SensorConfiguration conf)
        => string.Join(_topicSeparator, conf.DeviceId, conf.SensorId, "state");

    public static string GetAvalibilityTopic(this SensorConfiguration conf)
        => string.Join(_topicSeparator, conf.DeviceId, "avalibility");

    public static string GetAvalibilityTopic(this AvalibilityConfiguration conf)
        => string.Join(_topicSeparator, conf.DeviceId, "avalibility");

    public static string GetDiscoveryTopic(string id) => string.Join(_topicSeparator,
        "homeassistant", "sensor", id, "config");

    public static string GetFullId(this SensorConfiguration conf)
        => string.Join(_idSeparator, conf.DeviceId, conf.SensorId);

    public static string GetCapabilityId(this SensorConfiguration conf, string capabulityId)
        => string.Join(_idSeparator, conf.DeviceId, conf.SensorId, capabulityId);
}
