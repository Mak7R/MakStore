using MakStore.SharedComponents.Repositories;

namespace ProductsService.Models;

public class Product : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ResourcesUris { get; set; }
}