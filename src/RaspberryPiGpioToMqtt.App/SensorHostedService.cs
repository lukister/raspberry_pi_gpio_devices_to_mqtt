using Microsoft.Extensions.Options;
using RaspberryPoGpioToMqtt.Devices;

public class SensorsOptions
{
    public const string SectionName = "Sonsors";
    public TimeSpan SensorReadInterval { get; set; }
}

public class SensorHostedService(IOptions<SensorsOptions> options, IDeviceManager deviceRepository, ILogger<SensorHostedService> logger) : BackgroundService
{
    private SensorsOptions _sensorsOptions = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await deviceRepository.Initialize();
        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await deviceRepository.SendSensorStates();
            await Task.Delay(_sensorsOptions.SensorReadInterval, stoppingToken);
        }
    }
}
