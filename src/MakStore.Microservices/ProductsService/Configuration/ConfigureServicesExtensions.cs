using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://host.docker.internal:9011";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ProductsApiPolicy", b =>
            {
                b.Requirements.Add(new ClaimsAuthorizationRequirement("scope", ["products_api"]));
            });
        });
        
        services.AddControllers(options =>
        {
            options.Filters.Add(new AuthorizeFilter("ProductsApiPolicy"));
        });
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