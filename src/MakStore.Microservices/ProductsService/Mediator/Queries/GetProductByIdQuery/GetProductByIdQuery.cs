using MediatR;
using ProductsService.Dtos;

namespace ProductsService.Mediator.Queries.GetProductByIdQuery;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid ProductId { get; }

    public GetProductByIdQuery(Guid productId)
    {
        ProductId = productId;
    }
}