using Istu.Navigation.Infrastructure.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Istu.Navigation.Api.Filters;

//Оборачиваем ошибки привязки модели в свою модель ошбики
public class ValidationModelErrorWrapFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();
        if (!executedContext.ModelState.IsValid)
        {
            var errors = executedContext.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var modelError = CommonErrors.ValidationModelError(errors);
            executedContext.ModelState.Clear();
            executedContext.Result = new ObjectResult(modelError) { StatusCode = modelError.StatusCode };
        }
    }
}