using Microsoft.AspNetCore.Builder;
using Serilog;

namespace MakStore.SharedComponents.Logging;

public static class LoggingDependencyInjectionExtensions
{
    public static void UseSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services);
        });
    }
}