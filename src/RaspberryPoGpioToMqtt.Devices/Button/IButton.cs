namespace RaspberryPoGpioToMqtt.Devices.Button;

internal interface IButton
{
    public ButtonCapability[] GetCapabilities();
    Task Press(string stateData);
}
