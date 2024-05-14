using System.Diagnostics;

namespace Istu.Navigation.Api.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            var log = $"{context.Request.Method} {context.Request.Path} {stopwatch.ElapsedMilliseconds}ms , {context.Response.StatusCode}";
            if(context.Response.StatusCode >= 400)
                logger.LogWarning(log);
            else
            {
                logger.LogInformation(log);
            }
        }
    }
}