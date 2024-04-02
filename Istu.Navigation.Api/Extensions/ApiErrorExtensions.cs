using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Public.Models;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Extensions;

public static class ApiErrorExtensions
{
    public static ErrorDto ToErrorDto(this ApiError error)
    {
        return new ErrorDto
        {
            StatusCode = error.StatusCode,
            Message = error.Message,
            Urn = error.Urn
        };
    }
    
    public static IActionResult ToActionResult(this ApiError error)
    {
        return new ObjectResult(error.ToErrorDto())
        {
            StatusCode = error.StatusCode
        };
    }
}