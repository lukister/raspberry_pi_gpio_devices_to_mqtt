using RaspberryPoGpioToMqtt.Devices.Sensors;
using RaspberryPoGpioToMqtt.Devices.Configurations;
using RaspberryPoGpioToMqtt.Devices.DeviceRepository;

namespace RaspberryPoGpioToMqtt.Devices;

internal class DeviceManager : IAsyncDisposable, IDeviceManager
{
    private List<SensorCommunication> _sensors = [];
    private List<AvalibilityConfiguration> _avalibilityCofiguration = [];
    private readonly JsonFileDeviceRepository _repository;
    private readonly ICommunication _client;

    internal DeviceManager(JsonFileDeviceRepository repository, ICommunication client)
    {
        _repository = repository;
        _client = client;
    }
    public async Task Initialize()
    {
        InitializeConfiguration();
        await Task.Delay(1000);
        await SendInitializationMessages();
        await Task.Delay(1000);
        await SendAvaiabilityMessage(true);
        await Task.Delay(1000);
    }

    private async Task SendAvaiabilityMessage(bool isAvaiable)
    {
        var state = isAvaiable ? "online" : "offline";
        foreach (var avalibility in _avalibilityCofiguration)
        {
            await _client.Send(avalibility.GetAvalibilityTopic(), state);
        }
    }

    public async Task SendSensorStates()
    {
        foreach (var sensor in _sensors)
        {
            await sensor.SendSensorState();
        }
    }

    private void InitializeConfiguration()
    {
        var devicesConfiguration = _repository.GetAll();

        foreach (var device in devicesConfiguration)
        {
            _avalibilityCofiguration.Add(new AvalibilityConfiguration(device.Id));
            foreach (var sensor in device.Sensors)
            {
                var configuration = new SensorConfiguration(device.Id, sensor.Id, device.Name, sensor.Name);
                var sensorCommunication = new SensorCommunication(SensorFactory.Create(sensor), configuration, _client);
                _sensors.Add(sensorCommunication);
            }
        }
    }

    private async Task SendInitializationMessages()
    {
        foreach (var sensor in _sensors)
        {
            await sensor.SendConfigurationMessage();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await SendAvaiabilityMessage(false);
    }
}
