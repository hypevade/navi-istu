using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.OAuth.OauthApi)]
public class OAuthController : ControllerBase
{
    private readonly ILogger<OAuthController> logger;
    private readonly IIstuService istuService;

    public OAuthController(ILogger<OAuthController> logger, IIstuService istuService)
    {
        this.logger = logger;
        this.istuService = istuService;
    }

    [HttpGet(ApiRoutes.OAuth.AuthenticatePart)]
    public IActionResult AuthenticateUser()
    {
        var getUrl = istuService.GenerateRedirectUrl();
        if (getUrl.IsFailure)
            return getUrl.ApiError.ToActionResult();
        
        logger.LogInformation("User is redirected to: {AuthorizationUrl}", getUrl.Data);
        return Redirect(getUrl.Data);
    }

    [HttpGet(ApiRoutes.OAuth.CallBackPart)]
    public async Task<IActionResult> OAuthCallback(string code)
    {
        if (string.IsNullOrEmpty(code))
            return UsersApiErrors.CodeNotValidError().ToActionResult();

        var getTokenOperation = await istuService.ExchangeCodeForTokenAsync(code).ConfigureAwait(false);
        if (getTokenOperation.IsFailure)
            return getTokenOperation.ApiError.ToActionResult();

        var userInfo = await istuService.GetUserInfo(getTokenOperation.Data.AccessToken).ConfigureAwait(false);
        if (userInfo.IsFailure)
            return userInfo.ApiError.ToActionResult();

        var registerUserOperation = await istuService.RegisterIstuUser(userInfo.Data,
            getTokenOperation.Data.AccessToken,
            getTokenOperation.Data.RefreshToken);
        if (registerUserOperation.IsFailure)
            return registerUserOperation.ApiError.ToActionResult();

        HttpContext.Response.Headers.Append("Authorization", $"Bearer {registerUserOperation.Data.AccessToken}");
        HttpContext.Response.Headers.Append("Refresh", $"Bearer {registerUserOperation.Data.RefreshToken}");
        return Ok(registerUserOperation.Data);
    }
}