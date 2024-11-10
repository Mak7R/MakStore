using MakStore.Domain.Entities;
using MakStore.SharedComponents.Repositories;
using Microsoft.EntityFrameworkCore;
using ProductsService.Data;

namespace ProductsService.Infrastructure.Repositories;

public class ProductsRepository : Repository<Product, Guid>, IProductsRepository
{
    private readonly ProductsDbContext _dbContext;
    private readonly ILogger<Repository<Product, Guid>> _logger;

    public ProductsRepository(ProductsDbContext dbContext, ILogger<Repository<Product, Guid>> logger) : base(dbContext, logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Product?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Name == name, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }
}