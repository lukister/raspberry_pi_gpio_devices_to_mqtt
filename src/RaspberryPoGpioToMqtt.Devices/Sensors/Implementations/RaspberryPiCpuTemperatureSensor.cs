using Iot.Device.CpuTemperature;
using UnitsNet;

namespace RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;

internal class RaspberryPiCpuTemperatureSensor : ISensor
{
    private readonly CpuTemperature _temperature;

    public RaspberryPiCpuTemperatureSensor()
    {
        _temperature = new CpuTemperature();
    }
    public SensorCapability[] GetCapabilities()
    {
        var values = _temperature.ReadTemperatures();

        return values
            .Select(MapToSensorCapability)
            .ToArray();

    }

    private static SensorCapability MapToSensorCapability((string Sensor, Temperature Temperature) x)
        => new(x.Sensor.ToLower(), x.Sensor, "temperature", "{{ value_json." + x.Sensor.ToLower() +" }}", "°C");

    public Task<object> ReadState()
    {
        var temperatures = (object)ReadTemperatures();
        return Task.FromResult(temperatures);
    }

    internal Dictionary<string, double> ReadTemperatures()
    {
        var values = _temperature.ReadTemperatures();
        return values.ToDictionary(x => x.Sensor.ToLower(), x => x.Temperature.DegreesCelsius);
    }
}