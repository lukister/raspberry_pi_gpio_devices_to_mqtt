using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaspberryPoGpioToMqtt.Devices.DeviceRepository;

namespace RaspberryPoGpioToMqtt.Devices;

public interface ICommunication
{
    public Task Send(string topic, string message);
    public Task Send<T>(string topic, T message);
}

public static class ServiceExtension
{
    public static IServiceCollection AddDevices(this IServiceCollection serviceProvider, IConfiguration configuration)
    {
        serviceProvider.Configure<JsonFileDeviceRepository.Options>(
            configuration.GetSection(JsonFileDeviceRepository.Options.SectionName));
        serviceProvider.AddSingleton<JsonFileDeviceRepository>();
        serviceProvider.AddSingleton<IDeviceManager, DeviceManager>();
        return serviceProvider;
    }
}