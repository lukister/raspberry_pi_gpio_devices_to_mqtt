using Microsoft.Extensions.Options;
using System.Text.Json;

namespace RaspberryPoGpioToMqtt.Devices.DeviceRepository;

internal class JsonFileDeviceRepository
{
    private readonly Options _options;

    public class Options
    {
        public const string SectionName = "FileDeviceRespository";
        public string ConfigFilePath { get; set; } = "devices.json";
    }

    public JsonFileDeviceRepository(IOptions<Options> options)
    {
        _options = options.Value;
    }

    public List<DeviceEntity> GetAll()
    {
        using var fileStream = File.OpenRead(_options.ConfigFilePath);
        return JsonSerializer.Deserialize<List<DeviceEntity>>(fileStream)
            ?? throw new Exception();
    }
}
