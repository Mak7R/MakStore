using MakStore.Domain.Enums;

namespace OrdersService.Dtos;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime OrderedAt { get; set; }
    public OrderStatus Status { get; set; }
    public bool IsPaid { get; set; }
}