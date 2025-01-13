using System;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Consumer;

public class ConsumerFunction
{
    private readonly ILogger<ConsumerFunction> _logger;

    public ConsumerFunction(ILogger<ConsumerFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ConsumerFunction))]
    public void Run([ServiceBusTrigger("mytopic", "mysubscription", Connection = "")] ServiceBusReceivedMessage message)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        
        
        
        
    }
}