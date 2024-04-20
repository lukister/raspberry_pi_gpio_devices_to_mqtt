namespace RaspberryPoGpioToMqtt.Devices.Sensors;

internal interface ISensor
{
    public SensorCapability[] GetCapabilities();
    public Task<object> ReadState();
}
