using MakStore.Domain.Entities;
using MakStore.SharedComponents.Repositories;

namespace OrdersService.Infrastructure.Repositories;

public interface IOrdersRepository : IRepository<Order, Guid>
{
    
}