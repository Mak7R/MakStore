using MakStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProductsService.Data;

public class ProductsDbContext : DbContext
{
    public virtual DbSet<Product> Products { get; set; }

    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
    }
}