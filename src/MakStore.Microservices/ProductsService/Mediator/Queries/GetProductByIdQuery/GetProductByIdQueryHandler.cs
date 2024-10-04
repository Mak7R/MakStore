using MakStore.SharedComponents.Exceptions;
using MediatR;
using ProductsService.Dtos;
using ProductsService.Infrastructure.Repositories;
using ProductsService.Mapping;

namespace ProductsService.Mediator.Queries.GetProductByIdQuery;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductsRepository _productsRepository;

    public GetProductByIdQueryHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productsRepository.FindByIdAsync(request.ProductId, cancellationToken: cancellationToken);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {request.ProductId} not found.");

        return ApplicationMapper.Mapper.Map<ProductDto>(product);
    }
}