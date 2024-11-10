using MakStore.Domain.Entities;

namespace OrdersService.Mediator.Commands.HandleCreatedOrderCommand;

public class HandleCreatedOrderCommand
{
    public Order Order { get; set; }
}