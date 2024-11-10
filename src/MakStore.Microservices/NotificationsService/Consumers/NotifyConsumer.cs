using MakStore.SharedComponents.EventBus.Messages;
using MassTransit;

namespace NotificationsService.Consumers;

public class NotifyConsumer : IConsumer<OrderCreatedMessage>
{
    private readonly ILogger<NotifyConsumer> _logger;

    public NotifyConsumer(ILogger<NotifyConsumer> logger)
    {
        _logger = logger;
    }
    
    public Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        _logger.LogInformation("Handling created order: id:{id}; status: {status}", context.Message.OrderId, context.Message.Status);
        return Task.CompletedTask;
    }
}