using Asp.Versioning;
using AuthService.Configuration.Options;
using AuthService.Data;
using AuthService.Identity;
using AuthService.Interfaces;
using AuthService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace AuthService.Configuration;

public static class ConfigureServicesExtensions
{
    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var activeConnection = configuration["ActiveConnection"] ?? "DefaultConnection";
        var dbConnectionString = configuration.GetConnectionString(activeConnection) ??
                                 throw new InvalidOperationException(
                                     $"Connection string '{activeConnection}' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dbConnectionString));

        services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
                opt.User.RequireUniqueEmail = true;

                opt.Password.RequiredLength = 4;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IAuthTokenService<ApplicationUser>, JwtAuthTokenService>();
    }

    public static void ConfigureDefaultServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddHttpClient();
    }

    public static void ConfigureApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebGames API V1", Version = "1.0" });
        });
        
        services.AddApiVersioning(config =>
        {
            config.ApiVersionReader = new UrlSegmentApiVersionReader();
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
        })
        .AddApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'v'VVV";
            opt.SubstituteApiVersionInUrl = true;
        });
    }

    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtTokenOptions>(configuration.GetSection("Jwt"));
    }
}