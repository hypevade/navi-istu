/*using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Helpers;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Users;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Istu.Navigation.Api.Filters;

public class CustomAuthorizationFilter : Attribute, IAsyncAuthorizationFilter
{

    public CustomAuthorizationFilter()
    {
        
    }
    

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValue))
        {
            context.Result = UsersApiErrors.AuthorizationHeaderIsEmptyError().ToActionResult();
            return;
        }
        
        var token = JwtTokenHelper.ExtractToken(headerValue.ToString());
        if (token == null)
        {
            context.Result = UsersApiErrors.TokenIsNotValidError().ToActionResult();
            return;
        }
        
    }

    private async Task<OperationResult<Guid>> GetUserByToken(string token, AuthorizationFilterContext context)
    {
        var tokenHandler = context.HttpContext.RequestServices.GetRequiredService<IAccessTokenProvider>();
        var result = await tokenHandler.ValidateToken(token);
    }
}*/