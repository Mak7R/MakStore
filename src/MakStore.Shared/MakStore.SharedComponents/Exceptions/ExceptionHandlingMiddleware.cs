using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MakStore.SharedComponents.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerFactory _loggerFactory;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _loggerFactory = loggerFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            await HandleValidationExceptionAsync(context, validationException);
        }
        catch (NotFoundException notFoundException)
        {
            await HandleNotFoundExceptionAsync(context, notFoundException);
        }
        catch (AlreadyExistsException alreadyExistsException)
        {
            await HandleAlreadyExistsExceptionAsync(context, alreadyExistsException);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        Log(LogLevel.Information, "Validation failed while processing the request", exception);

        var details = new ValidationProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Invalid request data",
            Detail = "One or more fields contain invalid values. Please correct the highlighted errors and try again.",
            Instance = context.Request.Path,
            Errors = exception.Errors,
            Extensions = new Dictionary<string, object?>
            {
                {"errors", exception.Errors}
            }
        };
        
        return WriteResponseAsync(context, details);
    }

    private Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException exception)
    {
        Log(LogLevel.Information, "Resource not found while processing the request", exception);
        
        var details = new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = "Resource not found",
            Detail = "The requested resource could not be found. Please verify the input and try again.",
            Instance = context.Request.Path
        };
        
        return WriteResponseAsync(context, details);
    }

    private Task HandleAlreadyExistsExceptionAsync(HttpContext context, AlreadyExistsException exception)
    {
        Log(LogLevel.Information, "Duplicate resource found while processing the request", exception);
        
        var details = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Conflict,
            Title = "Resource conflict",
            Detail = "A resource with the same identifier already exists. Please use a unique identifier and try again.",
            Instance = context.Request.Path
        };
        
        return WriteResponseAsync(context, details);
    }
    
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Log(LogLevel.Error, "An unexpected error occurred while processing the request", exception);
        
        var details = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Unexpected error",
            Detail = "An unexpected error occurred. Please try again later or contact support if the issue persists.",
            Instance = context.Request.Path
        };

        return WriteResponseAsync(context, details);
    }

    private Task WriteResponseAsync(HttpContext context, ProblemDetails details)
    {
        context.Response.ContentType = "application/json";
        if (details.Status.HasValue)
            context.Response.StatusCode = details.Status.Value;
        else context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsJsonAsync(details);
    }
    
    private void Log(LogLevel level, string message, Exception? exception = null)
    {
        var logger = _loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
        if (exception == null)
        {
            logger.Log(level, message);
        }
        else
        {
            logger.Log(level, exception, message);
        }
    }
}

public static class ExceptionHandlingMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}