using System.Security.Claims;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Helpers;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Users;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Istu.Navigation.Api.Filters;

public class AuthorizationFilter(UserRole minimumRole) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        CheckAuthorizationHeader(context);
    }

    private void CheckAuthorizationHeader(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValue))
        {
            context.Result = UsersApiErrors.AuthorizationHeaderIsEmptyError().ToActionResult();
            return;
        }
        ValidateToken(context, headerValue.ToString());
    }

    private void ValidateToken(AuthorizationFilterContext context, string headerValue)
    {
        var token = JwtTokenHelper.ExtractToken(headerValue);
        if (token == null)
        {
            context.Result = UsersApiErrors.TokenIsNotValidError().ToActionResult();
            return;
        }

        var tokenHandler = context.HttpContext.RequestServices.GetRequiredService<IAccessTokenProvider>();
        var validateTokenAndGetUserId = tokenHandler.GetUser(token);

        if (validateTokenAndGetUserId.IsFailure)
        {
            context.Result = validateTokenAndGetUserId.ApiError.ToActionResult();
            return;
        }

        CheckRoleAndSetUser(context, validateTokenAndGetUserId.Data.Role, validateTokenAndGetUserId.Data.Id);
    }

    private void CheckRoleAndSetUser(AuthorizationFilterContext context, UserRole role, Guid userId)
    {
        if (role < minimumRole)
        {
            context.Result = UsersApiErrors.AccessDeniedError().ToActionResult();
            return;
        }

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Custom");
        context.HttpContext.User = new ClaimsPrincipal(identity);
    }
}