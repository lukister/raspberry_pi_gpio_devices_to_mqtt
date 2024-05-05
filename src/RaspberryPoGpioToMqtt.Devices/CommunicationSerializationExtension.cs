using System.Text.Json;

namespace RaspberryPoGpioToMqtt.Devices;

internal static class CommunicationSerializationExtension
{
    private static JsonSerializerOptions _serializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task Send<T>(this ICommunication communication, string topic, T message)
    {
        var payload = JsonSerializer.Serialize(message, _serializerOptions);
        await communication.Send(topic, payload);
    }

    public static async Task SubscribeFor<T>(this ICommunication communication, string topic, Func<T, Task> onMessageRecived)
    {
        await communication.SubscribeFor(topic, x =>
        {
            var message = JsonSerializer.Deserialize<T>(x)
                ?? throw new Exception("Unable to deserialize message");
            return onMessageRecived(message);
        });
    }

}
