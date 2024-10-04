namespace MakStore.SharedComponents.Repositories;

public abstract class Entity<TId>
{
    public TId Id { get; set; }
}