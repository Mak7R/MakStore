using MakStore.SharedComponents.Exceptions;
using MakStore.SharedComponents.Logging;
using ProductsService.Configuration;
using ProductsService.Data;
using Serilog;

namespace ProductsService;

public static class StartupExtension
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.UseSerilog();
        
        builder.Services.ConfigureBaseServices(builder.Configuration);
        builder.Services.ConfigureApi(builder.Configuration);
        builder.Services.ConfigureInfrastructure(builder.Configuration);
        builder.Services.ConfigureFeatures(builder.Configuration);
        
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

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        return app;
    }
}