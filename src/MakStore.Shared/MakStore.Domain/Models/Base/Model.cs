namespace MakStore.Domain.Models.Base;

public abstract class Model<TId>
{
    public TId Id { get; set; } = default;
}