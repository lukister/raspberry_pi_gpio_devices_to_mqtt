using RaspberryPiGpioToMqtt.App.DTOs;
using RaspberryPiGpioToMqtt.App.Sensors;

namespace RaspberryPiGpioToMqtt.App;

public class Device
{
    public Device(string id, string name, ISensor[] sensors)
    {
        Id = id;
        Name = name;
        Sensors = sensors;
    }

    public string Id { get; }
    public string Name { get; }
    public ISensor[] Sensors { get; }

    public SensorDto[] GetConfigurations()
    {
        var deviceConfiguration = new DeviceDto()
        {
            Identifiers = [Id],
            Name = Name,
        };

        var configurations = Sensors
            .SelectMany(x => x.GetConfigurations())
            .ToArray();

        foreach (var configuration in configurations)
            configuration.Device = deviceConfiguration;

        return configurations;
    }
}