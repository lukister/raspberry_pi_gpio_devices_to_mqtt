using Iot.Device.Bno055;
using Iot.Device.DHTxx;
using System.Device.Gpio;
using System.Diagnostics;
using System.Xml.Linq;

namespace RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;

internal class TestTemperatureAndHumiditySensor : BaseTemperatureAndHumiditySensor, ISensor
{
    public override Task<object> ReadState()
    {
        var random = new Random();

        var telemetry = new TemperatureAndHumidityTelemetryDto()
        {
            Temperature = random.Next(150, 220) / 10d,
            Humidity = random.Next(30, 60),
        };

        return Task.FromResult((object)telemetry);
    }
}

internal class Dht22TemperatureAndHumiditySensor : BaseTemperatureAndHumiditySensor, ISensor
{
    private readonly Dht22 _sensor;

    public Dht22TemperatureAndHumiditySensor(int pin, GpioController controller) 
        => _sensor = new Dht22(pin, gpioController: controller);

    public override Task<object> ReadState()
    {
        var result = new TemperatureAndHumidityTelemetryDto();

        var stopwathch = Stopwatch.StartNew();
        while (stopwathch.Elapsed < TimeSpan.FromSeconds(10))
        {
            if (TryReadTemperature(out var temperature))
            {
                if (temperature < -30 || temperature > 100)
                    continue;
                result.Temperature = temperature;
                break;
            }
        }

        while (stopwathch.Elapsed < TimeSpan.FromSeconds(10))
        {
            if (TryReadHumidity(out var humidity))
            {
                if (humidity < 0 || humidity > 100)
                    continue;
                result.Humidity = humidity;
                break;
            }
        }
        stopwathch.Stop();

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
