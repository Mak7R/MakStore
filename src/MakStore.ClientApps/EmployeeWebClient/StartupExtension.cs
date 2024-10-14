using System.Security.Claims;
using EmployeeWebClient.Configuration.Options;
using EmployeeWebClient.Middlewares;
using EmployeeWebClient.Services;
using IdentityModel;
using MakStore.SharedComponents.Configuration;
using MakStore.SharedComponents.Constants;
using MakStore.SharedComponents.Exceptions;
using MakStore.SharedComponents.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;

namespace EmployeeWebClient;

public static class StartupExtension
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        
        builder.UseSerilog();

        #region ConfigureOptions

        var oidcOptionsSection = configuration.GetSection("OidcOptions");
        var oidcOptions = oidcOptionsSection.Get<OidcOptions>() ?? throw new ConfigurationException("Oidc options was not configured");
        services.Configure<OidcOptions>(oidcOptionsSection);
        
        services.Configure<ServicesOptions>(builder.Configuration.GetSection("Services"));
        
        #endregion
        
        #region AddDefaultServices
        
        services.AddControllersWithViews();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/DataProtection-Keys"));

        #endregion

        #region AddAuthServices

        services.AddUserAccessTokenHttpClient(AutoAuthHttpClientDefaults.ClientName, configureClient: client =>
        {
            client.BaseAddress = new Uri(configuration["BaseAddress"] ?? throw new ConfigurationException("BaseAddress was not configured"));
        });
        services.AddOpenIdConnectAccessTokenManagement();
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OidcDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.HttpOnly = true;
            })
            .AddOpenIdConnect(OidcDefaults.AuthenticationScheme,options =>
            {
                options.Authority = oidcOptions.Authority;
                
                options.ClientId = oidcOptions.ClientId;
                options.ClientSecret = oidcOptions.ClientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                
                options.Scope.Clear();
                options.Scope.Add(OidcConstants.StandardScopes.OpenId); 
                options.Scope.Add(OidcConstants.StandardScopes.Profile);
                options.Scope.Add(OidcConstants.StandardScopes.Email);
                options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                
                options.Scope.Add(MicroservicesConstants.ApiDefaults.ProductsApi);
                options.Scope.Add("role");
                options.ClaimActions.MapJsonKey(ClaimTypes.Role, JwtClaimTypes.Role);
                
                options.GetClaimsFromUserInfoEndpoint = true;

                options.MapInboundClaims = false;
                options.DisableTelemetry = true;

                options.SaveTokens = true;
            });
        
        services.AddAuthorization();

        #endregion

        #region AddProjectServices

        services.AddScoped<IProductsService, ProductsService>();

        #endregion
        
        return builder;
    }

    public static WebApplication InitializeApp(this WebApplication app)
    {


        return app;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseMvcExceptionHandlingMiddleware();
        app.UseSerilogRequestLogging();

        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}