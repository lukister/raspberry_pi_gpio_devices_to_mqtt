using RaspberryPiGpioToMqtt.Devices.HomeAssistantMqttConfiguration;
using RaspberryPoGpioToMqtt.Devices.Configurations;

namespace RaspberryPoGpioToMqtt.Devices.Sensors;

internal class SensorCommunication
{
    private readonly ISensor _sensor;
    private readonly SensorConfiguration _configuration;
    private readonly ICommunication _communication;

    public SensorCommunication(ISensor sensor, SensorConfiguration configuration, ICommunication communication)
    {
        _sensor = sensor;
        _configuration = configuration;
        _communication = communication;
    }

    public async Task SendSensorState()
    {
        var state = _sensor.ReadState();
        await _communication.Send(_configuration.GetStateTopic(), state);
    }

    public async Task SendConfigurationMessage()
    {
        var device = new DeviceDto()
        {
            Name = _configuration.DeviceName,
            Identifiers = [_configuration.DeviceId]
        };

        var availabilityDto = new AvailabilityDto(_configuration.GetAvalibilityTopic());

        var capabilities = _sensor.GetCapabilities()
            .Select(x => new ConfigurationDto(_configuration.GetCapabilityId(x.Id))
            {
                Name = $"{_configuration.SensorName} {x.Name}",
                DeviceClass = x.DeviceClass,
                StateTopic = _configuration.GetStateTopic(),
                ValueTemplate = x.ValueTemplate,
                UnitOfMeasurement = x.UnitOfMeasurement,
                Device = device,
                Availability = [availabilityDto],
            })
            .ToArray();

        foreach (var capability in capabilities)
            await _communication.Send(ConfigurationExtension.GetDiscoveryTopic(capability.UniqueId), capability);
    }
}
