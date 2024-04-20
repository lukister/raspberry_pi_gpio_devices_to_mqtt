using RaspberryPoGpioToMqtt.Devices.Button.Implementations;
using RaspberryPoGpioToMqtt.Devices.DeviceRepository;
using RaspberryPoGpioToMqtt.Devices.Switch;

namespace RaspberryPoGpioToMqtt.Devices.Button;

internal static class ButtonFactory
{
    public static IButton Create(CapabilityEntity sensor) => sensor.Type switch
    {
        nameof(TestButton) => new TestButton(),
        _ => throw new Exception("Unknown sensor")//TODO custom exception
    };
}