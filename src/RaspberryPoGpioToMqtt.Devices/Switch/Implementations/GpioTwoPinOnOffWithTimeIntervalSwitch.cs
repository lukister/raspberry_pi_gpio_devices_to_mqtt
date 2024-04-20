using System.Device.Gpio;

namespace RaspberryPoGpioToMqtt.Devices.Switch.Implementations;

internal class GpioTwoPinOnOffWithTimeIntervalSwitch : ISwitch
{
    private const string _on = "ON";
    private const string _off = "OFF";

    private readonly int _pinOpen;
    private readonly int _pinClose;
    private readonly GpioController _controller;
    private readonly object _lockObj;
    private readonly TimeSpan _timeSpanToChange;

    public GpioTwoPinOnOffWithTimeIntervalSwitch(int pinOpen, int pinClose, TimeSpan? timeSpanToChange, GpioController controller)
    {
        _pinOpen = pinOpen;
        _pinClose = pinClose;
        _controller = controller;
        _lockObj = new object();
        _timeSpanToChange = timeSpanToChange ?? TimeSpan.FromSeconds(8);
        OpenPin(_pinOpen);
        OpenPin(_pinClose);
    }

    public SwitchCapability[] GetCapabilities()
    {
        var @switch = new SwitchCapability("switch", string.Empty, _on, _off);
        return [@switch];
    }

    public Task SetState(string stateData)
    {
        lock (_lockObj)
        {
            if (_off.Equals(stateData))
                Off();
            else if(_on.Equals(stateData))
                On();
        }

        return Task.CompletedTask;
    }

    private void OpenPin(int pin)
    {
        if (!_controller.IsPinOpen(pin))
            _controller.OpenPin(pin);
        _controller.SetPinMode(pin, PinMode.Output);
        _controller.Write(pin, PinValue.High);
    }

    public bool SetValue(string value)
    {
        
        
        return true;
    }

    private void On()
    {
        _controller.Write(_pinOpen, PinValue.Low);
        Thread.Sleep(_timeSpanToChange);
        _controller.Write(_pinOpen, PinValue.High);
    }

    private void Off()
    {
        _controller.Write(_pinClose, PinValue.Low);
        Thread.Sleep(_timeSpanToChange);
        _controller.Write(_pinClose, PinValue.High);
    }
}