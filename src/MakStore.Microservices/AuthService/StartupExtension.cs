using System.Net;
using Asp.Versioning;
using AuthService.Configuration.Options;
using AuthService.Data;
using AuthService.Identity;
using AuthService.Interfaces;
using AuthService.Middlewares;
using AuthService.Services;
using MakStore.SharedComponents.Exceptions;
using MakStore.SharedComponents.Logging;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace AuthService;

public static class StartupExtension
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var env = builder.Environment;
        
        builder.UseSerilog();

        #region ConfigureOptions

        var defaultConnection = configuration.GetConnectionString("DefaultConnection") ??
                                throw new ConfigurationException("Connection string 'DefaultConnection' was not found.");
        var identityServerDbConnectionString = configuration.GetConnectionString("IdentityServerDb")
                                               ?? throw new ConfigurationException("Connection string 'IdentityServerDb' was not found.");

        services.Configure<AdminTokenOptions>(configuration.GetSection("AdminOptions"));
        services.Configure<DevToolsOptions>(configuration.GetSection("DevTools"));
        
        #endregion
        
        #region AddInfrastructureServices
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(defaultConnection));

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
        
        services.AddScoped<IAdminTokenProvider, AdminTokenProvider>();

        #endregion
        
        #region AddDefaultServices

        services.AddControllersWithViews();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/DataProtection-Keys"));

        #endregion

        #region AddAuthServices

        services.ConfigureApplicationCookie(options => // !!! important: should be executed after AddIdentity because it overrides cookies options
        {
            options.Events.OnRedirectToLogin = async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = "Unauthorized",
                    Detail = "You do not have permission to access this resource.",
                    Instance = context.Request.Path,
                });
            };
            options.Events.OnRedirectToAccessDenied = async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Forbidden,
                    Title = "Access Denied",
                    Detail = "You do not have permission to access this resource.",
                    Instance = context.Request.Path,
                });
            };
        });
        services.AddAuthentication();
        services.AddAuthorization();

        #endregion
        
        #region AddApiServices

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "MakStore.AuthService API V1", Version = "1.0" });
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

        #endregion

        #region IdentityServer
        
        services.AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(identityServerDbConnectionString, 
                    sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(identityServerDbConnectionString, 
                    sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            })
            .AddAspNetIdentity<ApplicationUser>();

        #endregion
        
        return builder;
    }

    public static WebApplication InitializeApp(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Initialize(scope.ServiceProvider);
        }
        catch (Exception e)
        {
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
            logger.LogError(e, "An exception was thrown while initializing database");
        }

        return app;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseMvcExceptionHandlingMiddleware();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseIdentityServer();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}