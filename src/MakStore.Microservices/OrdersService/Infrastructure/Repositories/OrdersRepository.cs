using MakStore.Domain.Entities;
using MakStore.SharedComponents.Repositories;
using OrdersService.Data;

namespace OrdersService.Infrastructure.Repositories;

public class OrdersRepository : Repository<Order, Guid>, IOrdersRepository
{
    public OrdersRepository(OrdersDbContext dbContext, ILogger<Repository<Order, Guid>> logger) : base(dbContext, logger)
    {
    }
}