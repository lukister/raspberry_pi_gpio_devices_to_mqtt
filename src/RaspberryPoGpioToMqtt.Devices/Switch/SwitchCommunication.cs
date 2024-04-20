using RaspberryPiGpioToMqtt.Devices.HomeAssistantMqttConfiguration;
using RaspberryPoGpioToMqtt.Devices.Configurations;

namespace RaspberryPoGpioToMqtt.Devices.Switch;

internal class SwitchCommunication
{
    private readonly ISwitch _switch;
    private readonly CapabilityConfiguration _configuration;
    private readonly ICommunication _communication;

    public SwitchCommunication(ISwitch @switch, CapabilityConfiguration configuration, ICommunication communication)
    {
        _switch = @switch;
        _configuration = configuration;
        _communication = communication;
    }

    public async Task SendConfigurationMessage()
    {
        var device = new DeviceDto()
        {
            Name = _configuration.DeviceName,
            Identifiers = [_configuration.DeviceId]
        };

        var availabilityDto = new AvailabilityDto(_configuration.GetAvalibilityTopic());

        var capabilities = _switch.GetCapabilities()
            .Select(x => new ConfigurationDto(_configuration.GetCapabilityId(x.Id))
            {
                Name = $"{_configuration.SensorName} {x.Name}",
                StateTopic = _configuration.GetStateTopic(),
                CommandTopic = _configuration.GetCommandTopic(),
                Device = device,
                Availability = [availabilityDto],
                StateOn = x.StateOn,
                StateOff = x.StateOff,
                PayloadOn = x.StateOn,
                PayloadOff = x.StateOff,
                Optimistic = false,
            })
            .ToArray();

        foreach (var capability in capabilities)
        {
            await _communication.Send(ConfigurationExtension.GetSwitchDiscoveryTopic(capability.UniqueId), capability);
            await _communication.SubscribeFor(capability.CommandTopic!, async message =>
            {
                await _switch.SetState(message);
                await _communication.Send(capability.StateTopic!, message);
            });
        }
    }
}