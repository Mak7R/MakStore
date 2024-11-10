using MakStore.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MakStore.SharedComponents.Repositories;

public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity: Entity<TId>
{
    protected const string DefaultOnExceptionMessage = "An exception was thrown while processing the request to database";

    private readonly DbContext _dbContext;
    private readonly ILogger<Repository<TEntity, TId>> _logger;

    protected Repository(DbContext dbContext, ILogger<Repository<TEntity, TId>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }
    
    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity, TId>.GetQuery(_dbContext.Set<TEntity>().AsNoTracking().AsQueryable(), spec);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ApplySpecification(spec).ToListAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, DefaultOnExceptionMessage);
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
            _logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }
    
    public virtual async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(e => e.Id != null && e.Id.Equals(id))
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception e)
        {
            _logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception e)
        {
            _logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }

    public virtual async Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception e)
        {
            _logger.LogError(e, DefaultOnExceptionMessage);
            throw;
        }
    }
}