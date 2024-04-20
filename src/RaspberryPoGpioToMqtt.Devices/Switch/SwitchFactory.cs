using RaspberryPoGpioToMqtt.Devices.DeviceRepository;
using RaspberryPoGpioToMqtt.Devices.Switch.Implementations;

namespace RaspberryPoGpioToMqtt.Devices.Switch;
internal static class SwitchFactory
{
    public static ISwitch Create(CapabilityEntity sensor) => sensor.Type switch
    {
        //TODO get from additional data
        nameof(TestSimpleSwitch) => new TestSimpleSwitch(),
        _ => throw new Exception("Unknown sensor")//TODO custom exception
    };
}