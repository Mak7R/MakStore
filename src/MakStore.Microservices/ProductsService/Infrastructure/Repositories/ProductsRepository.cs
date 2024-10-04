using MakStore.SharedComponents.Repositories;
using Microsoft.EntityFrameworkCore;
using ProductsService.Data;
using ProductsService.Models;

namespace ProductsService.Infrastructure.Repositories;

public class ProductsRepository : Repository<ProductsDbContext, Product, Guid>, IProductsRepository
{
    public ProductsRepository(ProductsDbContext dbContext, ILogger<Repository<ProductsDbContext, Product, Guid>> logger) : base(dbContext, logger)
    {
    }

    public async Task<Product?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbContext.Products.FirstOrDefaultAsync(p => p.Name == name, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }
}