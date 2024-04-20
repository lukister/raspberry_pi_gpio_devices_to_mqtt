using System.Device.Gpio;

namespace RaspberryPoGpioToMqtt.Devices.Switch.Implementations;

internal class GpioOnOffSwitch : ISwitch
{
    private const string _on = "ON";
    private const string _off = "OFF";

    private readonly int _pin;
    private readonly bool _lowMeanOn;
    private readonly GpioController _controller;
    private readonly PinValue _onValue;
    private readonly PinValue _offValue;

    public GpioOnOffSwitch(int pin, bool lowMeanOn, GpioController controller)
    {
        _pin = pin;
        _lowMeanOn = lowMeanOn;
        _controller = controller;

        if (_lowMeanOn)
        {
            _onValue = PinValue.Low;
            _offValue = PinValue.High;
        }
        else
        {
            _onValue = PinValue.High;
            _offValue = PinValue.Low;
        }

        if (!_controller.IsPinOpen(_pin))
            _controller.OpenPin(_pin);
        _controller.SetPinMode(_pin, PinMode.Output);
    }

    public SwitchCapability[] GetCapabilities()
    {
        var @switch = new SwitchCapability("switch", string.Empty, _on, _off);
        return [@switch];
    }

    public Task SetState(string stateData)
    {
        if (_on.Equals(stateData))
            On();
        else if (_off.Equals(stateData))
            Off();

        return Task.CompletedTask;
    }

    private void Off()
        => _controller.Write(_pin, _offValue);

    private void On()
        => _controller.Write(_pin, _onValue);
}