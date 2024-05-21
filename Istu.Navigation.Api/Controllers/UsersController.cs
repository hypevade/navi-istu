using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Users;
using Istu.Navigation.Public.Models.Users;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Istu.Navigation.Public.Models.Users.LoginRequest;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Users.UsersApi)]
public class UsersController(IUsersService usersService, IMapper mapper, ILogger<UsersController> logger) : ControllerBase
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
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var loginUser =
            await usersService.LoginUser(request.Email, request.Password)
                .ConfigureAwait(false);

        if (loginUser.IsFailure)
        {
            var apiError = loginUser.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        
        var response = new LoginResponse
        {
            User = mapper.Map<UserDto>(loginUser.Data),
            AccessToken = loginUser.Data.AccessToken,
            RefreshToken = loginUser.Data.RefreshToken
        };
        
        return Ok(response);
    }
    
    [HttpGet]
    [Route(ApiRoutes.Users.GetUserInfo)]
    [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<UserDto>> GetUserInfo()
    {
        var userId = HttpContext.GetUserId(logger);
        if (userId is null)
            return Unauthorized();

        var getUserOperation =
            await usersService.GetUserInfo(userId.Value)
                .ConfigureAwait(false);

        if (getUserOperation.IsFailure)
        {
            var apiError = getUserOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<UserDto>(getUserOperation.Data));
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