

namespace MakStore.SharedComponents.EventBus.Messages;

public class OrderCreatedMessage
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime OrderedAt { get; set; }
    public string Status { get; set; }
}