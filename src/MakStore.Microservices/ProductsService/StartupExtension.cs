using Asp.Versioning;
using FluentValidation;
using MakStore.SharedComponents.Constants;
using MakStore.SharedComponents.Exceptions;
using MakStore.SharedComponents.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductsService.Data;
using ProductsService.Infrastructure.Repositories;
using Serilog;

namespace ProductsService;

public static class StartupExtension
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        builder.UseSerilog();
        
        services.AddControllers(options =>
        {
            options.Filters.Add(new AuthorizeFilter("ProductsApiPolicy"));
        });
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
                b.Requirements.Add(new ClaimsAuthorizationRequirement("scope", [MicroservicesConstants.ApiDefaults.ProductsApi]));
            });
        });
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
        var defaultConnection = configuration.GetConnectionString("DefaultConnection") ??
                                 throw new ConfigurationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseNpgsql(defaultConnection));

        services.AddScoped<IProductsRepository, ProductsRepository>();
        var programAssembly = typeof(Program).Assembly;
        
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(programAssembly);
        });
        services.AddValidatorsFromAssembly(programAssembly);
        services.AddAutoMapper(programAssembly);
        
        return builder;
    }

    public static WebApplication InitializeApp(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
            dbContext.Initialize(app.Configuration);
        }
        catch (Exception e)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProductsDbContext>>();
            logger.LogError(e, "An exception was thrown while db initializing");
        }
        
        return app;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseExceptionHandlingMiddleware();
        app.UseSerilogRequestLogging();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHttpsRedirection();
        }
        
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        return app;
    }
}