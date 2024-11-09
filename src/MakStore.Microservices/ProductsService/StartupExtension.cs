using Asp.Versioning;
using FluentValidation;
using MakStore.SharedComponents.Configuration;
using MakStore.SharedComponents.Constants;
using MakStore.SharedComponents.Exceptions;
using MakStore.SharedComponents.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductsService.Configuration;
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

        #region ConfigureOptions

        var currentAssembly = typeof(Program).Assembly;
        var defaultConnection = configuration.GetConnectionString("DefaultConnection") ??
                                throw new ConfigurationException("Connection string 'DefaultConnection' not found.");

        var jwtOidcOptionsSection = configuration.GetSection("JwtOidcOptions");
        var jwtOidcOptions = jwtOidcOptionsSection.Get<JwtOidcOptions>() ?? throw new ConfigurationException("JwtOidcOptions was not configured");
        services.Configure<JwtOidcOptions>(jwtOidcOptionsSection);
        
        #endregion
        
        #region AddDefaultServices

        services.AddControllers(options =>
        {
            options.Filters.Add(new AuthorizeFilter(PolicyDefaults.ProductsApiPolicy));
        });
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/DataProtection-Keys"));

        #endregion


        #region AddAuthServices

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = jwtOidcOptions.Authority;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyDefaults.ProductsApiPolicy, b =>
            {
                b.Requirements.Add(new ClaimsAuthorizationRequirement(MicroservicesConstants.ApiDefaults.Scope, [MicroservicesConstants.ApiDefaults.ProductsApi]));
            });
        });

        #endregion

        #region AddApiServices

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
            
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

        #endregion

        #region AddInfrastructureServices
        
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseNpgsql(defaultConnection));

        services.AddScoped<IProductsRepository, ProductsRepository>();

        #endregion

        #region AddFeatures

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(currentAssembly);
        });
        services.AddValidatorsFromAssembly(currentAssembly);
        services.AddAutoMapper(currentAssembly);

        #endregion
        
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
        app.UseSerilogRequestLogging();
        app.UseExceptionHandlingMiddleware();
        
        
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

        app.UseCors();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        return app;
    }
}