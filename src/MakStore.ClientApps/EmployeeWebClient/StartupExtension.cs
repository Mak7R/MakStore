using MakStore.SharedComponents.Authentication;
using MakStore.SharedComponents.Authentication.Options;
using MakStore.SharedComponents.Exceptions;
using MakStore.SharedComponents.Logging;
using Serilog;

namespace EmployeeWebClient;

public static class StartupExtension
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.UseSerilog();
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient();

        builder.Services.Configure<MicroservicesAuthenticationOptions>(builder.Configuration.GetSection("Authentication"));

        builder.Services.AddAuthentication(MicroservicesAuthenticationDefaults.AuthenticationScheme)
            .AddScheme<MicroservicesAuthenticationOptions, MicroservicesAuthenticationHandler>(MicroservicesAuthenticationDefaults.AuthenticationScheme,
                opt =>
                {
                    opt.ValidateAccessTokenUrl = builder.Configuration.GetSection("Authentication")
                        .Get<MicroservicesAuthenticationOptions>()?.ValidateAccessTokenUrl ?? throw new InvalidOperationException("ValidateAccessTokenUrl is required");
                    opt.AccessTokenProvider = context => context.Request.Cookies["AccessToken"];
                });
    

        builder.Services.AddAuthorization();

        return builder;
    }

    public static WebApplication InitializeApp(this WebApplication app)
    {


        return app;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseExceptionHandlingMiddleware();
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

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