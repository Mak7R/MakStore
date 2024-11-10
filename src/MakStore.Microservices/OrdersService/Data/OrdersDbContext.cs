using MakStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrdersService.Data;

public class OrdersDbContext : DbContext
{
    public virtual DbSet<Order> Orders { get; set; }
    
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
    }
}