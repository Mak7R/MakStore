using Microsoft.EntityFrameworkCore;

namespace ProductsService.Data;

public static class DbInitializer
{
    public static void Initialize(this ProductsDbContext dbContext, IConfiguration configuration)
    {
        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
        }
    }
}