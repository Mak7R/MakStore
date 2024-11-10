namespace MakStore.Domain.Entities.Base;

public abstract class Entity<TId>
{
    public TId Id { get; set; }
}