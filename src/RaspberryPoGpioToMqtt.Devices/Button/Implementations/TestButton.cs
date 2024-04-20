namespace RaspberryPoGpioToMqtt.Devices.Button.Implementations;
internal class TestButton : IButton
{
    public ButtonCapability[] GetCapabilities()
    {
        var capability = new ButtonCapability("button", string.Empty);
        return [capability];
    }

    public Task Press(string stateData)
    {
        Console.WriteLine($"State set to {stateData}");
        return Task.CompletedTask;
    }
}
