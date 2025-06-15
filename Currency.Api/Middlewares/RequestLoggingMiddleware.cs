using System.Diagnostics;
using System.Security.Claims;
using Serilog.Context;

namespace Currency.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        var method = context.Request.Method;
        var path = context.Request.Path;
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "undefined";
        var requestHost = context.Request.Host.ToString();
        var scheme = context.Request.Scheme;
        var requestId = context.TraceIdentifier;
        var clientId = context.User?.Claims.FirstOrDefault(c => 
            c.Type == ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        using (LogContext.PushProperty("ClientIP", clientIp))
        using (LogContext.PushProperty("RequestHost", requestHost))
        using (LogContext.PushProperty("RequestScheme", scheme))
        using (LogContext.PushProperty("RequestPath", path))
        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("ClientId", clientId))
        {
            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();

                var statusCode = context.Response.StatusCode;
                var responseTimeMs = sw.ElapsedMilliseconds;

                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms; " +
                    "Client IP: {ClientIp}; ClientId: {ClientId}",
                    method, path, statusCode, responseTimeMs, clientIp, clientId);
            }
        }
    }
}

