using AuthService.Configuration;
using AuthService.Identity;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using MakStore.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public static class DbInitializer
{
    public static void Initialize(this ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        dbContext.Database.Migrate();

        if (false) 
            InitializeIdentityServerDatabase(serviceProvider);
        else 
            ReinitializeIdentityServerDatabase(serviceProvider);
        
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        roleManager.InitializeRolesAsync().Wait();
    }

    private static async Task InitializeRolesAsync(this RoleManager<ApplicationRole> roleManager)
    {
        var roleNames = Enum.GetNames<UserRole>();
        foreach (var roleName in roleNames)
        {
            if (await roleManager.FindByNameAsync(roleName) == null)
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }
    }
    
    private static void InitializeIdentityServerDatabase(IServiceProvider serviceProvider)
    {
        var persistedGrantDbContext = serviceProvider.GetRequiredService<PersistedGrantDbContext>();
        persistedGrantDbContext.Database.Migrate();
        
        var context = serviceProvider.GetRequiredService<ConfigurationDbContext>();
        context.Database.Migrate();
        if (!context.Clients.Any())
        {
            foreach (var client in IdentityServerConfig.Clients)
            {
                context.Clients.Add(client.ToEntity());
            }
            context.SaveChanges();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in IdentityServerConfig.IdentityResources)
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var resource in IdentityServerConfig.ApiScopes)
            {
                context.ApiScopes.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }
    }

    private static void ReinitializeIdentityServerDatabase(IServiceProvider serviceProvider)
    {
        var persistedGrantDbContext = serviceProvider.GetRequiredService<PersistedGrantDbContext>();
        persistedGrantDbContext.Database.Migrate();
        
        var context = serviceProvider.GetRequiredService<ConfigurationDbContext>();
        context.Database.Migrate();

        context.Clients.RemoveRange(context.Clients.ToList());
        context.IdentityResources.RemoveRange(context.IdentityResources.ToList());
        context.ApiScopes.RemoveRange(context.ApiScopes.ToList());
        
        context.SaveChanges();
        
        foreach (var client in IdentityServerConfig.Clients)
            context.Clients.Add(client.ToEntity());
        foreach (var resource in IdentityServerConfig.IdentityResources)
            context.IdentityResources.Add(resource.ToEntity());
        foreach (var resource in IdentityServerConfig.ApiScopes)
            context.ApiScopes.Add(resource.ToEntity());
        context.SaveChanges();
    }
}