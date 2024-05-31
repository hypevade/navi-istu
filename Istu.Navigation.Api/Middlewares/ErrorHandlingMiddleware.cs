using Istu.Navigation.Infrastructure.Errors;
using Newtonsoft.Json;

namespace Istu.Navigation.Api.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = CommonErrors.InternalServerError();

        var result = JsonConvert.SerializeObject(error);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = error.StatusCode;
        return context.Response.WriteAsync(result);
    }
}
