namespace RaspberryPoGpioToMqtt.Devices.Sensors;

internal record Capability(string Id, string Name, string DeviceClass, string ValueTemplate, string? UnitOfMeasurement);
