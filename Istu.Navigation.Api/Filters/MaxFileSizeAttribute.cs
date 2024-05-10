using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Infrastructure.Errors;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Istu.Navigation.Api.Filters;

public class MaxFileSizeAttribute(long fileSize) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var files = httpContext.Request.Form.Files;

        foreach (var file in files)
        {
            if (file.Length > fileSize)
            {
                context.Result = CommonErrors.FileTooLarge(file.Length, fileSize).ToActionResult();
                return;
            }
        }
        base.OnActionExecuting(context);
    }
}
