using MakStore.Domain.Entities;
using MediatR;

namespace OrdersService.Mediator.Events.OrderCreatedEvent;

public class OrderCreatedEvent : INotification
{
    public Order Order { get; set; }
}