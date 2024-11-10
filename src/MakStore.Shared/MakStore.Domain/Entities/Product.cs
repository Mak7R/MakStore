using MakStore.Domain.Entities.Base;

namespace MakStore.Domain.Entities;

public class Product : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ResourcesUris { get; set; }
}