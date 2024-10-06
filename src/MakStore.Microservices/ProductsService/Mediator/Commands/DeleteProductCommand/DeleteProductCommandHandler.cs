using MakStore.SharedComponents.Exceptions;
using MediatR;
using ProductsService.Infrastructure.Repositories;

namespace ProductsService.Mediator.Commands.DeleteProductCommand;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductsRepository _productsRepository;

    public DeleteProductCommandHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productsRepository.FindByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new NotFoundException($"Product with id {request.Id} was not found");

        await _productsRepository.DeleteAsync(product, cancellationToken);
    }
}