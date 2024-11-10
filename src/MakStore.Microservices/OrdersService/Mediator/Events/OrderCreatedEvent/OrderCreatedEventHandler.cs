using MediatR;

namespace OrdersService.Mediator.Events.OrderCreatedEvent;

public class OrderCreatedEventLoggingHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEvent> _logger;

    public OrderCreatedEventLoggingHandler(ILogger<OrderCreatedEvent> logger)
    {
        _logger = logger;
    }
    
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order '{orderId}' status changed to {status}", notification.Order.Id, notification.Order.Status.ToString());
    }
}