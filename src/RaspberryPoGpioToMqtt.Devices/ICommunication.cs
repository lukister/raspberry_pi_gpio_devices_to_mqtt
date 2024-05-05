using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaspberryPoGpioToMqtt.Devices.DeviceRepository;

namespace RaspberryPoGpioToMqtt.Devices;

public interface ICommunication
{
    Task Send(string topic, string message);
    Task SubscribeFor(string topic, Func<string, Task> onMessageRecived);
}
public static class ServiceExtension
{
    public static IServiceCollection AddDevices(this IServiceCollection serviceProvider, IConfiguration configuration)
    {
        serviceProvider.Configure<JsonFileDeviceRepository.Options>(
            configuration.GetSection(JsonFileDeviceRepository.Options.SectionName));
        serviceProvider.AddSingleton<JsonFileDeviceRepository>();
        serviceProvider.AddSingleton<IDeviceManager, DeviceManager>();
        serviceProvider.AddSingleton<Factory>();
        return serviceProvider;
    }
}