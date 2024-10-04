using AuthService.Data.Configuration;
using AuthService.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
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
        builder.Entity<IdentityUserClaim<Guid>>(opt => opt.ToTable("UserClaims"));
        builder.Entity<IdentityUserRole<Guid>>(opt => opt.ToTable("UserRoles"));
        builder.Entity<IdentityUserLogin<Guid>>(opt => opt.ToTable("UserLogins"));
        builder.Entity<IdentityRoleClaim<Guid>>(opt => opt.ToTable("RoleClaims"));
        builder.Entity<IdentityUserToken<Guid>>(opt => opt.ToTable("UserTokens"));
    }
}