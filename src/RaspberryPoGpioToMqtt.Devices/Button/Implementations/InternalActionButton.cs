namespace RaspberryPoGpioToMqtt.Devices.Button.Implementations;

internal class InternalActionButton : IButton
{
    private readonly Func<Task> _onPressAction;

    public InternalActionButton(Func<Task> onPressAction)
    {
        _onPressAction = onPressAction;
    }
    public ButtonCapability[] GetCapabilities()
    {
        var capability = new ButtonCapability("button", string.Empty);
        return [capability];
    }

    public async Task Press(string stateData)
    {
        await _onPressAction();
    }
}