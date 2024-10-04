using Asp.Versioning;
using FluentValidation;
using MakStore.SharedComponents.Authentication;
using MakStore.SharedComponents.Authentication.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductsService.Data;
using ProductsService.Infrastructure.Repositories;

namespace ProductsService.Configuration;

public static class ConfigureServicesExtensions
{
    public static void ConfigureBaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddHttpClient();
        services.AddAuthentication()
            .AddMicroservicesAuthentication(options =>
            {
                options.ValidateAccessTokenUrl = configuration.GetSection("Authentication")
                    .Get<MicroservicesAuthenticationOptions>()?.ValidateAccessTokenUrl ?? throw new InvalidOperationException("ValidateAccessTokenUrl is required");
                options.AccessTokenProvider = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                    var minLength = "Bearer ".Length;
                    
                    if (authHeader == null || authHeader.Length < minLength) return null;

                    return authHeader.Substring(minLength);
                };
            });
        services.AddAuthorization();
    }
    
    public static void ConfigureApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "MakStore.ProductsService API V1", Version = "1.0" });
        });
        
        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    public static void ConfigureFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        var programAssembly = typeof(Program).Assembly;
        
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(programAssembly);
        });
        services.AddValidatorsFromAssembly(programAssembly);
        services.AddAutoMapper(programAssembly);
    }
    
    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var activeConnection = configuration["ActiveConnection"] ?? "DefaultConnection";
        var dbConnectionString = configuration.GetConnectionString(activeConnection) ??
                                 throw new InvalidOperationException(
                                     $"Connection string '{activeConnection}' not found.");
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseNpgsql(dbConnectionString));

        services.AddScoped<IProductsRepository, ProductsRepository>();
    }
}