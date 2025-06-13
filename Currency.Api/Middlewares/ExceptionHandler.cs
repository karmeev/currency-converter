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

        // try
        // {
        //     await next(context);
        // }
        // catch (AuthenticationException authException)
        // {
        //     await HandleAuthenticationException(authException);
        // }
        // catch (ValidationException validationException)
        // {
        //     await HandleValidationException(validationException);
        // }
    }

    // private async Task Handle(Exception exception, HttpStatusCode statusCode)
    // {
    //     logger.LogInf(exception.Message);
    //
    //     _context.Response.StatusCode = (int)statusCode;
    //
    //     await _context.Response.WriteAsJsonAsync(new
    //     {
    //         exception.Message,
    //     });
    // }
    //
    // private async Task Handle(Exception exception, HttpStatusCode statusCode, string uiMessage)
    // {
    //     logger.LogInf(exception.Message);
    //
    //     _context.Response.StatusCode = (int)statusCode;
    //
    //     await _context.Response.WriteAsJsonAsync(new
    //     {
    //         Message = exception.Message,
    //         UIMessage = uiMessage,
    //     });
    // }
    //
    // private async Task HandleAuthenticationException(AuthenticationException authenticationException)
    // {
    //     logger.LogInf(authenticationException.Message);
    //
    //     _context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //
    //     await _context.Response.WriteAsJsonAsync(new
    //     {
    //         Message = authenticationException.Message,
    //         UIMessage = authenticationException.UIMessage,
    //     });
    // }
    //
    // private async Task HandleValidationException(ValidationException validationException)
    // {
    //     logger.LogWrn(validationException.Message);
    //
    //     _context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    //
    //     await _context.Response.WriteAsJsonAsync(new
    //     {
    //         validationException.Message,
    //         validationException.UIMessage,
    //         validationException.ValidationSource
    //     });
    // }
}