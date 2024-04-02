using Istu.Navigation.Infrastructure.Errors.Errors;
using Newtonsoft.Json;

namespace Istu.Navigation.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
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
