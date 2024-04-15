namespace RaspberryPoGpioToMqtt.Devices.Sensors;

internal interface ISensor
{
    public Capability[] GetCapabilities();
    public object ReadState();
}
