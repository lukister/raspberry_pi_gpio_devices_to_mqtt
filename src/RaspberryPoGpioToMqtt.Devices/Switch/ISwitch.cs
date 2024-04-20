using RaspberryPiGpioToMqtt.Devices;

namespace RaspberryPoGpioToMqtt.Devices.Switch;

internal interface ISwitch
{
    public SwitchCapability[] GetCapabilities();
    Task SetState(string stateData);
}
