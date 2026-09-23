using System.Diagnostics;

namespace UserManagementAPI.Middleware;

public sealed class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Incoming request {Method} {Path}", context.Request.Method, context.Request.Path);

        context.Response.OnCompleted(() =>
        {
            stopwatch.Stop();
            logger.LogInformation("Outgoing response {Method} {Path} {StatusCode} in {ElapsedMilliseconds} ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            return Task.CompletedTask;
        });

        await next(context);
    }
}