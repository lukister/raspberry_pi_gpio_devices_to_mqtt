namespace RaspberryPoGpioToMqtt.Devices;

public interface IDeviceManager
{
    ValueTask DisposeAsync();
    Task Initialize();
    Task SendSensorStates();
}