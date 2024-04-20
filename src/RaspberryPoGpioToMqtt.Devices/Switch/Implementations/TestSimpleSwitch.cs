namespace RaspberryPoGpioToMqtt.Devices.Switch.Implementations;

internal class TestSimpleSwitch(string? stateOn = null, string? stateOff = null) : ISwitch
{
    private readonly string? _stateOn = stateOn;
    private readonly string? _stateOff = stateOff;

    public SwitchCapability[] GetCapabilities()
    {
        var @switch = new SwitchCapability("switch", string.Empty, _stateOn ?? "ON", _stateOff ?? "OFF");
        return [@switch];
    }

    public Task SetState(string message)
    {
        Console.WriteLine($"State set to {message}");
        return Task.CompletedTask;
    }
}