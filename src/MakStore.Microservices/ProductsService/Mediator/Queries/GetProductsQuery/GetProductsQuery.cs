using MediatR;
using ProductsService.Dtos;

namespace ProductsService.Mediator.Queries.GetProductsQuery;

public class GetProductsQuery : IRequest<IEnumerable<ProductDto>>
{
    
}