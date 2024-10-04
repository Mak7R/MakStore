using MakStore.Domain.Models.Base;

namespace MakStore.Domain.Models;

public class Product : Model<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}