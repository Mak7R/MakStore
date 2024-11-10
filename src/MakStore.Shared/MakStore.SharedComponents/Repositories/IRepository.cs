using MakStore.Domain.Entities.Base;

namespace MakStore.SharedComponents.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity: Entity<TId>
{
    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default);

    public Task<IReadOnlyList<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    public Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}