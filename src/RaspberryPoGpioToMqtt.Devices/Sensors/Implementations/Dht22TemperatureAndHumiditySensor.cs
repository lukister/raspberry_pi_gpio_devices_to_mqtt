using Iot.Device.DHTxx;
using System.Device.Gpio;
using System.Diagnostics;

namespace RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;

internal class Dht22TemperatureAndHumiditySensor : BaseTemperatureAndHumiditySensor, ISensor
{
    private readonly Dht22 _sensor;

    public Dht22TemperatureAndHumiditySensor(int pin, GpioController controller) 
        => _sensor = new Dht22(pin, gpioController: controller);

    public override Task<object> ReadState()
    {
        var result = new TemperatureAndHumidityTelemetryDto();
        bool temperatureRead = false, humidityRead = false;
        var stopwathch = Stopwatch.StartNew();
        while (stopwathch.Elapsed < TimeSpan.FromSeconds(10))
        {
            temperatureRead = TryReadTemperature(out var temperature);
            if (temperatureRead)
            {
                if (temperature < -30 || temperature > 100)
                    continue;
                result.Temperature = Math.Round(temperature, 1);
                break;
            }
            Thread.Sleep(_sensor.MinTimeBetweenReads);
        }
        
        while (stopwathch.Elapsed < TimeSpan.FromSeconds(10))
        {
            humidityRead = TryReadHumidity(out var humidity);
            if (humidityRead)
            {
                if (humidity < 0 || humidity > 100)
                    continue;
                result.Humidity = Math.Round(humidity, 0);
                break;
            }
            Thread.Sleep(_sensor.MinTimeBetweenReads);
        }
        stopwathch.Stop();
        if (!humidityRead || !temperatureRead)
            throw new Exception("Unable to read temperature and humidity in 10 second.");
        return Task.FromResult((object)result);
    }

    private bool TryReadTemperature(out double temperatureCelsius)
    {
        if (_sensor.TryReadTemperature(out var reading))
        {
            temperatureCelsius = reading.DegreesCelsius;
            return true;
        }

        temperatureCelsius = default;
        return false;
    }

    private bool TryReadHumidity(out double humidityPercent)
    {
        if (_sensor.TryReadHumidity(out var reading))
        {
            humidityPercent = reading.Percent;
            return true;
        }

        humidityPercent = default;
        return false;
    }
}
