using MakStore.SharedComponents.Repositories;
using ProductsService.Models;

namespace ProductsService.Infrastructure.Repositories;

public interface IProductsRepository : IRepository<Product, Guid>
{
    Task<Product?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}