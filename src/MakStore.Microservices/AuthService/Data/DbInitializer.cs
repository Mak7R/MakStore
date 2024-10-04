using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public static class DbInitializer
{
    public static void Initialize(this ApplicationDbContext dbContext)
    {
        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
        }
    }
}