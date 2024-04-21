using RaspberryPoGpioToMqtt.Devices.DeviceRepository;
using RaspberryPoGpioToMqtt.Devices.Sensors;
using RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;
using RaspberryPoGpioToMqtt.Devices.Switch.Implementations;
using RaspberryPoGpioToMqtt.Devices.Switch;
using System.Device.Gpio;

namespace RaspberryPoGpioToMqtt.Devices;
internal class Factory
{
    Lazy<GpioController> _controller = new Lazy<GpioController>(() => new GpioController());

    public Factory()
    {
    }

    public ISensor CreateSensor(CapabilityEntity sensor) => sensor.Type switch
    {
        nameof(TestTemperatureAndHumiditySensor)
            => new TestTemperatureAndHumiditySensor(),
        nameof(Dht22TemperatureAndHumiditySensor)
            => new Dht22TemperatureAndHumiditySensor(sensor.GetValue<int>("Pin"), _controller.Value),
        _ => throw new Exception("Unknown sensor")//TODO custom exception
    };

    public ISwitch CreateSwitch(CapabilityEntity sensor) => sensor.Type switch
    {
        nameof(TestSimpleSwitch)
            => new TestSimpleSwitch(),
        nameof(GpioOnOffSwitch)
            => new GpioOnOffSwitch(sensor.GetValue<int>("Pin"), sensor.GetValue<bool>("LowMeanOn"), _controller.Value),
        nameof(GpioTwoPinOnOffWithTimeIntervalSwitch)
            => new GpioTwoPinOnOffWithTimeIntervalSwitch(
                sensor.GetValue<int>("PinOpen"), sensor.GetValue<int>("PinClose"),
                sensor.GetValue<TimeSpan>("TimeSpanToChange"), _controller.Value),
        _ => throw new Exception("Unknown sensor")//TODO custom exception
    };
}
