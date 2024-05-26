using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Istu.Navigation.Public.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.OAuthRoutes.OauthApi)]
public class OAuthController : ControllerBase
{
    private readonly ILogger<OAuthController> logger;
    private readonly IIstuService istuService;
    private readonly IMapper mapper;

    public OAuthController(ILogger<OAuthController> logger, IIstuService istuService, IMapper mapper)
    {
        this.logger = logger;
        this.istuService = istuService;
        this.mapper = mapper;
    }

    [HttpGet(ApiRoutes.OAuthRoutes.AuthenticatePart)]
    public IActionResult AuthenticateUser()
    {
        var getUrl = istuService.GenerateRedirectUrl();
        if (getUrl.IsFailure)
            return getUrl.ApiError.ToActionResult();
        
        logger.LogInformation("User is redirected to: {AuthorizationUrl}", getUrl.Data);
        return Redirect(getUrl.Data);
    }

    [HttpGet(ApiRoutes.OAuthRoutes.CallBackPart)]
    public async Task<ActionResult<LoginResponse>> OAuthCallback(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            var error = UsersApiErrors.CodeNotValidError();
            return StatusCode(error.StatusCode,error.ToErrorDto());
        }

        var getOperation = await istuService.ExchangeCodeForTokenAsync(code).ConfigureAwait(false);
        if (getOperation.IsFailure)
            return StatusCode(getOperation.ApiError.StatusCode,getOperation.ApiError.ToErrorDto());

        var userInfo = await istuService.GetUserInfo(getOperation.Data.AccessToken).ConfigureAwait(false);
        if (userInfo.IsFailure)
            return StatusCode(userInfo.ApiError.StatusCode,userInfo.ApiError.ToErrorDto());

        var registerUserOperation = await istuService.RegisterIstuUser(userInfo.Data,
            getOperation.Data.AccessToken,
            getOperation.Data.RefreshToken);
        
        if (registerUserOperation.IsFailure)
            return StatusCode(registerUserOperation.ApiError.StatusCode,registerUserOperation.ApiError.ToErrorDto());
        

        HttpContext.Response.Headers.Append("Authorization", $"Bearer {registerUserOperation.Data.AccessToken}");
        HttpContext.Response.Headers.Append("Refresh", $"Bearer {registerUserOperation.Data.RefreshToken}");
        var response = new LoginResponse
        {
            User = mapper.Map<UserDto>(registerUserOperation.Data),
            AccessToken = registerUserOperation.Data.AccessToken!,
            RefreshToken = registerUserOperation.Data.RefreshToken!
        };
            
        return Ok(response);
    }
}