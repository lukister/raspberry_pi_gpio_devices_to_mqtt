using RaspberryPiGpioToMqtt.Devices.HomeAssistantMqttConfiguration;
using RaspberryPoGpioToMqtt.Devices.Configurations;

namespace RaspberryPoGpioToMqtt.Devices.Button;

internal class ButtonCommunication
{
    private readonly IButton _button;
    private readonly CapabilityConfiguration _configuration;
    private readonly ICommunication _communication;

    public ButtonCommunication(IButton button, CapabilityConfiguration configuration, ICommunication communication)
    {
        _button = button;
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

        var capabilities = _button.GetCapabilities()
            .Select(x => new ConfigurationDto(_configuration.GetCapabilityId(x.Id))
            {
                Name = $"{_configuration.SensorName} {x.Name}",
                CommandTopic = _configuration.GetCommandTopic(),
                Device = device,
                Availability = [availabilityDto],
                Optimistic = false,
            })
            .ToArray();

        foreach (var capability in capabilities)
        {
            await _communication.Send(ConfigurationExtension.GetButtonDiscoveryTopic(capability.UniqueId), capability);
            await _communication.SubscribeFor(capability.CommandTopic!, async message =>
            {
                await _button.Press(message);
            });
        }
    }
}
