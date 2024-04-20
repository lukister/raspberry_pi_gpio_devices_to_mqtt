namespace RaspberryPoGpioToMqtt.Devices.Switch.Implementations;


internal class TestSimpleSwitch : ISwitch
{
    public SwitchCapability[] GetCapabilities()
    {
        var @switch = new SwitchCapability($"onOff", "On off switch", "ON", "OFF");
        return [@switch];
    }

    public Task SetState(string message)
    {
        Console.WriteLine($"State set to {message}");
        return Task.CompletedTask;
    }
}