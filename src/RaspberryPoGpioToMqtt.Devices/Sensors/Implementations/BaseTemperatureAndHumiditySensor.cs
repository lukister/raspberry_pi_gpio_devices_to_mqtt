namespace RaspberryPoGpioToMqtt.Devices.Sensors.Implementations;

internal abstract class BaseTemperatureAndHumiditySensor : ISensor
{
    public SensorCapability[] GetCapabilities()
    {
        var tempSensor = new SensorCapability("temperature", "Temperature", "temperature",
            "{{ value_json.temperature }}", "°C");

        var humidytySenspr = new SensorCapability("humidity", "Humidity", "humidity",
            "{{ value_json.humidity }}", "%");

        return [tempSensor, humidytySenspr];
    }

    public abstract Task<object> ReadState();
}
