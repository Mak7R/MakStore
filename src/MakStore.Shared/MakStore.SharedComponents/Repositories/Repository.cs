using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MakStore.SharedComponents.Repositories;

public abstract class Repository<TDbContext, TEntity, TId> : IRepository<TEntity, TId>
    where TEntity: Entity<TId>
    where TDbContext: DbContext
{
    protected const string DefaultOnExceptionMessage = "An exception was thrown while processing the request to database";
    
    protected readonly TDbContext DbContext;
    protected readonly ILogger<Repository<TDbContext, TEntity, TId>> Logger;

    protected Repository(TDbContext dbContext, ILogger<Repository<TDbContext, TEntity, TId>> logger)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        Logger = logger;
    }
    
    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity, TId>.GetQuery(DbContext.Set<TEntity>().AsNoTracking().AsQueryable(), spec);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ApplySpecification(spec).ToListAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }
    
    public virtual async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ApplySpecification(spec).CountAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }
    
    public virtual async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(e => e.Id != null && e.Id.Equals(id))
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await DbContext.Set<TEntity>().AddAsync(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception e)
        {
            Logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception e)
        {
            Logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }

    public virtual async Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            DbContext.Set<TEntity>().Remove(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception e)
        {
            Logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }
}