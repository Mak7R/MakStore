using MediatR;
using ProductsService.Dtos;
using ProductsService.Infrastructure.Repositories;
using ProductsService.Infrastructure.Specification;
using ProductsService.Mapping;

namespace ProductsService.Mediator.Queries.GetProductsQuery;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductsRepository _productsRepository;

    public GetProductsQueryHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }
    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var result = await _productsRepository.GetAsync(ProductSpecification.Empty, cancellationToken);
        return ApplicationMapper.Mapper.Map<IEnumerable<ProductDto>>(result);
    }
}