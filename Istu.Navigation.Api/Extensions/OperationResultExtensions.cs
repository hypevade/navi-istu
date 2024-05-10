using Istu.Navigation.Infrastructure.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Extensions;

public static class OperationResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this OperationResult<T> operationResult)
    {
        if (operationResult.IsFailure)
        {
            var apiError = operationResult.ApiError;
            return new ObjectResult(apiError.ToErrorDto())
            {
                StatusCode = apiError.StatusCode
            };
        }
        return new OkObjectResult(operationResult.Data);
    }
    
    
    
    
}