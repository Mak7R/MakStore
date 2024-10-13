using AuthService.Data.Configuration;
using AuthService.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfiguration).Assembly);
        builder.Entity<ApplicationUser>(opt => opt.ToTable("Users"));
        builder.Entity<ApplicationRole>(opt => opt.ToTable("Roles"));
        builder.Entity<IdentityUserClaim<string>>(opt => opt.ToTable("UserClaims"));
        builder.Entity<IdentityUserRole<string>>(opt => opt.ToTable("UserRoles"));
        builder.Entity<IdentityUserLogin<string>>(opt => opt.ToTable("UserLogins"));
        builder.Entity<IdentityRoleClaim<string>>(opt => opt.ToTable("RoleClaims"));
        builder.Entity<IdentityUserToken<string>>(opt => opt.ToTable("UserTokens"));
    }
}