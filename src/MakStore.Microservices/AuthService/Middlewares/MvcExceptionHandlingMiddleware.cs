using System.Net;

namespace AuthService.Middlewares;

public class MvcExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerFactory _loggerFactory;

    public MvcExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _loggerFactory = loggerFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                context.Response.Redirect($"/account/login?returnUrl={context.Request.Path}");
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.Redirect("/account/login"); // access denied
            }
        }
        catch (Exception e)
        {
            var logger = _loggerFactory.CreateLogger<MvcExceptionHandlingMiddleware>();
            logger.LogError(e, "An unexpected error occurred while processing the request");
            
            context.Response.Redirect("/Home/Error");
        }
    }
}

public static class MvcExceptionHandlingMiddlewareExtension
{
    public static IApplicationBuilder UseMvcExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MvcExceptionHandlingMiddleware>();
    }
}