using MakStore.SharedComponents.Repositories;

namespace OrdersService.Models;

public class Order : Entity<Guid>
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime OrderedAt { get; set; }
    public OrderStatus Status { get; set; }
    public bool IsPaid { get; set; }
}