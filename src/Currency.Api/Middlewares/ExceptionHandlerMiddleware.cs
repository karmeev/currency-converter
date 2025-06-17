using System.Net;
using System.Security.Claims;
using Currency.Api.Models;
using Currency.Api.Schemes;
using Currency.Facades.Contracts.Exceptions;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;
using Serilog.Context;

namespace Currency.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            await HandleValidationException(context, validationException);
        }
        catch (HttpProviderException providerException)
        {
            await HandleHttpProviderException(context, providerException);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleHttpProviderException(HttpContext context, HttpProviderException ex)
    {
        Log(context, ex);
        
        context.Response.StatusCode = (int)ex.StatusCode;
        await context.Response.WriteAsJsonAsync(new ErrorResponseScheme
        {
            Error = ErrorMessage.InvalidExternalApiResponse,
            Message = ex.Message,
            Details = BuildDetails(context, ex)
        });
    }
    
    private async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        Log(context, ex);
        
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsJsonAsync(new ErrorResponseScheme
        {
            Error = ErrorMessage.ValidationError,
            Message = ex.Message,
            Details = ex.ErrorMessages
        });
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        Log(context, ex);

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsJsonAsync(new ErrorResponseScheme
        {
            Error = ErrorMessage.InternalServerError,
            Message = "An unexpected error occurred while processing your request.",
            Details = BuildDetails(context, ex)
        });
    }

    private void Log(HttpContext context, Exception ex)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "undefined";
        var requestId = context.TraceIdentifier;
        var clientId = context.User?.Claims.FirstOrDefault(c => 
            c.Type == ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        using (LogContext.PushProperty("ClientIP", clientIp))
        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("ClientId", clientId))
        {
            _logger.LogError(ex, "Error occurred: {Message}. TraceId: {TraceId}", ex.Message, requestId);
        }
    }

    private object BuildDetails(HttpContext context, Exception ex, string[] errors = null)
    {
        var env =  Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env is "Development") return BuildDevelopmentDetails(context, ex, errors);
        
        if (errors != null)
        {
            return new
            {
                TraceId = context.TraceIdentifier,
                Errors = errors
            };
        }
        
        return new
        {
            TraceId = context.TraceIdentifier,
        };
    }

    private static object BuildDevelopmentDetails(HttpContext context, Exception ex, string[] errors)
    {
        if (errors != null)
        {
            return new
            {
                TraceId = context.TraceIdentifier,
                Errors = errors,
                Message = ex.Message,
                StackTrace = ex.StackTrace
            };
        }
        
        return new
        {
            TraceId = context.TraceIdentifier,
            Message = ex.Message,
            StackTrace = ex.StackTrace,
        };
    }
}
