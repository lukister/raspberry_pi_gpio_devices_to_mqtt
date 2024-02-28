using Microsoft.Extensions.Options;
using RaspberryPiGpioToMqtt.App.Sensors;
using System.Text.Json;

namespace RaspberryPiGpioToMqtt.App;

public class DeviceRepository
{
    public class Options
    {
        public const string SectionName = "FileDeviceRespository";
        public string ConfigFilePath { get; set; } = "devices.json";
    }

    private readonly List<Device> _devices = [];
    public IReadOnlyList<Device> GetDevices() => _devices;

    public DeviceRepository(IOptions<Options> options)
    {
        Cofiguration configuration = ReadConfigurationFromFile(options);
        foreach (var device in configuration.Devices)
        {
            var sensors = new ISensor[device.Sensors.Length];
            for (int i = 0; i < sensors.Length; i++)
            {
                var sensor = device.Sensors[i];
                var id = string.Join("_", configuration.SystemId, device.Id, sensor.Id);
                var topicPrefix = string.Join("/", configuration.SystemId, device.Id, sensor.Id);
                //TODO swith on type
                sensors[i] = new TestTemperatureAndHumiditySensor(id, topicPrefix, sensor.Name);
            }
            _devices.Add(new Device(string.Join("_", configuration.SystemId, device.Id), device.Name, sensors));
        }
    }



    private static Cofiguration ReadConfigurationFromFile(IOptions<Options> options)
    {
        using var fileStream = File.OpenRead(options.Value.ConfigFilePath);
        return JsonSerializer.Deserialize<Cofiguration>(fileStream)
            ?? throw new Exception();
        //TODO exception
    }

    private class Cofiguration
    {
        public string SystemId { get; set; } = string.Empty;
        public DeviceConfig[] Devices { get; set; } = [];
    }

    private class DeviceConfig
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public SensorConfig[] Sensors { get; set; } = [];
    }

    public class SensorConfig
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
