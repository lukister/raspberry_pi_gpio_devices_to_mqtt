namespace RaspberryPoGpioToMqtt.Devices.Sensors;

internal record SensorCapability(string Id, string Name, string DeviceClass, string ValueTemplate, string? UnitOfMeasurement);
