using RaspberryPoGpioToMqtt.Devices;

namespace RaspberryPiGpioToMqtt.App.MQTT;

public class CommunicationLogger : ICommunication
{
    private readonly ICommunication _communication;
    private readonly ILogger<CommunicationLogger> _logger;

    public CommunicationLogger(ICommunication communication, ILogger<CommunicationLogger> logger)
    {
        _communication = communication;
        _logger = logger;
    }

    public async Task Send(string topic, string message)
    {
        try
        {
            await _communication.Send(topic, message);
            _logger.LogInformation("Message send. Topic: {Topic} Body: {Body}", topic, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Massage send failded. Topic: {Topic} Body: {Body}", topic, message);
        }
    }

    public async Task SubscribeFor(string topic, Func<string, Task> onMessageRecived)
    {
        await _communication.SubscribeFor(topic, async x =>
        {
            try
            {
                await onMessageRecived(x);
                _logger.LogInformation("Recived and handled message. Topic: {Topic} Body: {Body}", topic, x);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandlex message. Topic: {Topic} Body: {Body}", topic, x);
            }
        });
    }
}