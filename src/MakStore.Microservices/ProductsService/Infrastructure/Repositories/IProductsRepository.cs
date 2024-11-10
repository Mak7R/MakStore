using MakStore.Domain.Entities;
using MakStore.SharedComponents.Repositories;

namespace ProductsService.Infrastructure.Repositories;

public interface IProductsRepository : IRepository<Product, Guid>
{
    Task<Product?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}