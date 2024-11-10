using MediatR;

namespace OrdersService.Mediator.Commands.CreateOrderCommand;

public class CreateOrderCommand : IRequest<Guid>
{
    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }
}