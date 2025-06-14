using System.Net;
using Currency.Api.Schemes;
using Currency.Facades.Contracts.Exceptions;

namespace Currency.Api.Middlewares;

public static class ExceptionHandlerExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandler>();
    }
}

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IMiddleware
{
    private HttpContext _context;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _context = context;

        try
        {
            await next(context);
        }
        catch (ValidationException validationException)
        {
            await HandleValidationException(validationException);
        }
    }
    
    private async Task HandleValidationException(ValidationException validationException)
    {
        //TODO: add LogWrn with message
    
        _context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    
        await _context.Response.WriteAsJsonAsync(new ErrorResponseScheme
        {
            Error = "validation_failed",
            Message = validationException.Message,
            Details = validationException.ErrorMessages
        });
    }
}