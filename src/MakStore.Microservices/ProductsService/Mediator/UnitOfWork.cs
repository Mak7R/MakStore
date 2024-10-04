using MakStore.SharedComponents.Mediator;
using ProductsService.Data;

namespace ProductsService.Mediator;

public class UnitOfWork : IUnitOfWork
{
    private readonly ProductsDbContext _dbContext;

    public UnitOfWork(ProductsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Rollback()
    {
        // todo 
        // or
        // ignore
    }
}