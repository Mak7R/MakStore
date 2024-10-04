using MakStore.SharedComponents.Repositories;
using ProductsService.Models;

namespace ProductsService.Infrastructure.Specification;

public class ProductSpecification : BaseSpecification<Product>
{
    public static readonly ProductSpecification Empty = new ();
}