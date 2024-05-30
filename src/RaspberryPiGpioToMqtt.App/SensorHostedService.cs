using Microsoft.Extensions.Options;
using RaspberryPoGpioToMqtt.Devices;

public class SensorsOptions
{
    public const string SectionName = "Sonsors";
    public TimeSpan SensorReadInterval { get; set; }
}

public class SensorHostedService(IOptions<SensorsOptions> options, 
    IDeviceManager deviceRepository, ILogger<SensorHostedService> logger,
    IHostApplicationLifetime  hostApplicationLifetime) 
    : BackgroundService
{
    private SensorsOptions _sensorsOptions = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        hostApplicationLifetime.ApplicationStopped.Register(() =>
        {
            logger.LogInformation("Application Stopped");
        });

        hostApplicationLifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("Application Stopping");
        });

        await deviceRepository.Initialize();
        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogDebug("Reading sensor values");
                await deviceRepository.SendSensorStates();
                await Task.Delay(_sensorsOptions.SensorReadInterval, stoppingToken);
            }
            catch (Exception exception) 
            {
                logger.LogError(exception, "Read sensors failed.");
            }
        }
    }
}