using RaspberryPoGpioToMqtt.Devices.Sensors;
using RaspberryPoGpioToMqtt.Devices.Configurations;
using RaspberryPoGpioToMqtt.Devices.DeviceRepository;
using RaspberryPoGpioToMqtt.Devices.Switch;
using RaspberryPoGpioToMqtt.Devices.Button;
using Microsoft.Extensions.Logging;

namespace RaspberryPoGpioToMqtt.Devices;

internal class DeviceManager : IAsyncDisposable, IDeviceManager
{
    private List<SensorCommunication> _sensors = [];
    private List<SwitchCommunication> _switches = [];
    private List<ButtonCommunication> _buttons = [];
    private List<AvalibilityConfiguration> _avalibilityCofiguration = [];
    private readonly JsonFileDeviceRepository _repository;
    private readonly ICommunication _client;
    private readonly Factory _factory;
    private readonly ILogger<DeviceManager> _logger;

    public DeviceManager(JsonFileDeviceRepository repository, ICommunication client, Factory factory, ILogger<DeviceManager> logger)
    {
        _repository = repository;
        _client = client;
        _factory = factory;
        _logger = logger;
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
            try
            {
                await sensor.SendSensorState();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception during sensor read");
            }
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
                var sensorCommunication = new SensorCommunication(_factory.CreateSensor(sensor), configuration, _client);
                _sensors.Add(sensorCommunication);
            }

            foreach(var @switch in device.Switches)
            {
                var configuration = new CapabilityConfiguration(device.Id, @switch.Id, device.Name, @switch.Name);
                var switchCommunication = new SwitchCommunication(_factory.CreateSwitch(@switch), configuration, _client);
                _switches.Add(switchCommunication);
            }

            foreach (var button in device.Buttons)
            {
                var configuration = new CapabilityConfiguration(device.Id, button.Id, device.Name, button.Name);
                var buttonCommunication = new ButtonCommunication(ButtonFactory.Create(button), configuration, _client);
                _buttons.Add(buttonCommunication);
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

        foreach (var button in _buttons)
        {
            await button.SendConfigurationMessage();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await SendAvaiabilityMessage(false);
    }
}
