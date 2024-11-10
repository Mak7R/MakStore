using MakStore.SharedComponents.EventBus.Messages;
using MassTransit;
using MediatR;

namespace OrdersService.Mediator.Events.OrderCreatedEvent;

public class OrderCreatedEventNotificationHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedEventNotificationHandler(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        var order = notification.Order;
        var message = new OrderCreatedMessage
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            OrderedAt = order.OrderedAt,
            Status = order.Status.ToString()
        };

        await _publishEndpoint.Publish(message, cancellationToken);
    }
}