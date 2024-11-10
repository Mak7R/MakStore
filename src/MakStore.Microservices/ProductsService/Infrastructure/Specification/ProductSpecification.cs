using MakStore.Domain.Entities;
using MakStore.SharedComponents.Repositories;

namespace ProductsService.Infrastructure.Specification;

public class ProductSpecification : BaseSpecification<Product>
{
    public static readonly ProductSpecification Empty = new ();
}