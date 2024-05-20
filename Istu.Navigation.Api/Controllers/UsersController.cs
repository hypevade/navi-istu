using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services.Users;
using Istu.Navigation.Public.Models.Users;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Istu.Navigation.Public.Models.Users.LoginRequest;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Users.UsersApi)]
public class UsersController(IUsersService usersService, IMapper mapper) : ControllerBase
{
    private readonly IUsersService usersService = usersService;
    private readonly IMapper mapper = mapper;

    [HttpPost]
    [Route(ApiRoutes.Users.RegisterPart)]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserRequest request)
    {
        var createUserOperation =
            await usersService.RegisterUser(request.Email, request.Password, request.FirstName, request.LastName)
                .ConfigureAwait(false);

        if (createUserOperation.IsFailure)
        {
            var apiError = createUserOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<UserDto>(createUserOperation.Data));
    }

    [HttpPost]
    [Route(ApiRoutes.Users.LoginPart)]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
    {
        var loginUser =
            await usersService.LoginUser(request.Email, request.Password)
                .ConfigureAwait(false);

        if (loginUser.IsFailure)
        {
            var apiError = loginUser.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(loginUser.Data);
    }
    
    [HttpPost]
    [Route(ApiRoutes.Users.RefreshPart)]
    public async Task<ActionResult<RefreshTokenResult>> RefreshToken()
    {
        var refreshToken =
            await usersService.RefreshToken(Request.Headers["RefreshToken"].ToString())
                .ConfigureAwait(false);

        return refreshToken.IsSuccess
            ? Ok(new RefreshTokenResult { AccessToken = refreshToken.Data.accessToken, RefreshToken = refreshToken.Data.refreshToken })
            : StatusCode(refreshToken.ApiError.StatusCode, refreshToken.ApiError.ToErrorDto());
    }
}