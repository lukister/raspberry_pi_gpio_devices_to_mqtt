using RaspberryPoGpioToMqtt.Devices;

public class SensorHostedService(IDeviceManager deviceRepository) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await deviceRepository.Initialize();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            Console.WriteLine("Reading Sensors");
            await deviceRepository.SendSensorStates();
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
