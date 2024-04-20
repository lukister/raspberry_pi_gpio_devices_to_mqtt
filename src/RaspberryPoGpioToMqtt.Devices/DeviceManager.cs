using RaspberryPoGpioToMqtt.Devices.Sensors;
using RaspberryPoGpioToMqtt.Devices.Configurations;
using RaspberryPoGpioToMqtt.Devices.DeviceRepository;
using RaspberryPoGpioToMqtt.Devices.Switch;

namespace RaspberryPoGpioToMqtt.Devices;

internal class DeviceManager : IAsyncDisposable, IDeviceManager
{
    private List<SensorCommunication> _sensors = [];
    private List<SwitchCommunication> _switches = [];
    private List<AvalibilityConfiguration> _avalibilityCofiguration = [];
    private readonly JsonFileDeviceRepository _repository;
    private readonly ICommunication _client;

    public DeviceManager(JsonFileDeviceRepository repository, ICommunication client)
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
                var configuration = new CapabilityConfiguration(device.Id, sensor.Id, device.Name, sensor.Name);
                var sensorCommunication = new SensorCommunication(SensorFactory.Create(sensor), configuration, _client);
                _sensors.Add(sensorCommunication);
            }

            foreach(var swich in device.Switches)
            {
                var configuration = new CapabilityConfiguration(device.Id, swich.Id, device.Name, swich.Name);
                var switchCommunication = new SwitchCommunication(SwitchFactory.Create(swich), configuration, _client);
                _switches.Add(switchCommunication);
            }
        }
    }

    private async Task SendInitializationMessages()
    {
        foreach (var sensor in _sensors)
        {
            await sensor.SendConfigurationMessage();
        }

        foreach (var @switch in _switches)
        {
            await @switch.SendConfigurationMessage();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await SendAvaiabilityMessage(false);
    }
}
