using AuthService.Data;

namespace AuthService.Configuration.Extensions;

public static class StartupExtension
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var env = builder.Environment;

        services.ConfigureOptions(configuration);
        services.ConfigureDefaultServices(configuration);
        services.ConfigureInfrastructure(configuration);
        services.ConfigureApi(configuration);
        
        return builder;
    }

    private static void InitializeApp(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Initialize();
        }
        catch (Exception e)
        {
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
            logger.LogError(e, "An exception was thrown while initializing database");
        }
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        var services = app.Services;
        var configuration = app.Configuration;
        var env = app.Environment;

        services.InitializeApp();
        
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


        
        
        app.UseRouting();
        
        app.UseAuthorization();

        app.MapControllers();
        
        return app;
    }
}