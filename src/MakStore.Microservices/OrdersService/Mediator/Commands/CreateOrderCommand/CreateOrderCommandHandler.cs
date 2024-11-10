using MakStore.Domain.Entities;
using MakStore.Domain.Enums;
using MakStore.SharedComponents.Exceptions;
using MediatR;
using OrdersService.Infrastructure.Repositories;
using OrdersService.Interfaces;
using OrdersService.Mapping;
using OrdersService.Mediator.Events.OrderCreatedEvent;

namespace OrdersService.Mediator.Commands.CreateOrderCommand;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrdersRepository _repository;
    private readonly IProductsServiceClient _productsServiceClient;
    private readonly IUsersServiceClient _usersServiceClient;
    private readonly IMediator _mediator;

    public CreateOrderCommandHandler(IOrdersRepository repository, IProductsServiceClient productsServiceClient, IUsersServiceClient usersServiceClient, IMediator mediator)
    {
        _repository = repository;
        _productsServiceClient = productsServiceClient;
        _usersServiceClient = usersServiceClient;
        _mediator = mediator;
    }
    
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // todo validate

        var checkUserTask = _usersServiceClient.CheckUserExists(request.CustomerId);
        var checkProductTask = _productsServiceClient.CheckProductExists(request.ProductId);

        await Task.WhenAll(checkUserTask, checkProductTask);

        var isUserValid = await checkUserTask;
        var isProductValid = await checkProductTask;
        
        if (!isUserValid)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(request.CustomerId), [$"Customer with id '{request.CustomerId}' does not exist"] }
            });
        }

        if (!isProductValid)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(request.ProductId), [$"Product with id '{request.ProductId}' does not exist"] }
            });
        }
        
        var order = ApplicationMapper.Mapper.Map<Order>(request);
        order.OrderedAt = DateTime.UtcNow;
        order.Status = OrderStatus.Registered;

        var result = await _repository.CreateAsync(order, cancellationToken);

        await _mediator.Publish(new OrderCreatedEvent { Order = result }, cancellationToken);
        
        return result.Id;
    }
}