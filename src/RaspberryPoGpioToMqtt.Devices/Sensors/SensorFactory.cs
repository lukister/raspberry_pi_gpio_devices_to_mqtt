using RaspberryPoGpioToMqtt.Devices.DeviceRepository;
using RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;

namespace RaspberryPoGpioToMqtt.Devices.Sensors;
internal static class SensorFactory
{
    public static ISensor Create(SensorEntity sensor) => sensor.Type switch
    {
        nameof(TestTemperatureAndHumiditySensor) => new TestTemperatureAndHumiditySensor(),
        _ => throw new Exception("Unknown sensor")//TODO custom exception
    };
}
