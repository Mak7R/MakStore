using System.Security.Claims;
using EmployeeWebClient.Configuration.Options;
using EmployeeWebClient.Middlewares;
using EmployeeWebClient.Services;
using IdentityModel;
using MakStore.SharedComponents.Constants;
using MakStore.SharedComponents.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;

namespace EmployeeWebClient;

public static class StartupExtension
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.UseSerilog();
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();


        builder.Services.AddUserAccessTokenHttpClient("apiClient", configureClient: client =>
        {
            client.BaseAddress = new Uri("http://localhost:9002");
        });
        builder.Services.AddOpenIdConnectAccessTokenManagement();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.HttpOnly = true;
            })
            .AddOpenIdConnect("oidc",options =>
            {
                options.Authority = "https://host.docker.internal:9011";
                
                options.ClientId = "EmployeesWebClient";
                options.ClientSecret = "secret";
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
            

        builder.Services.Configure<ServicesOptions>(builder.Configuration.GetSection("Services"));

        builder.Services.AddAuthorization();

        builder.Services.AddScoped<IProductsService, ProductsService>();
        
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