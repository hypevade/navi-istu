using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Helpers;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Users;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Istu.Navigation.Public.Models.Users;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Istu.Navigation.Public.Models.Users.LoginRequest;

namespace Istu.Navigation.Api.Controllers;

[ApiController] [Route(ApiRoutes.UsersRoutes.UsersApi)]
public class UsersController : ControllerBase
{
    private readonly IUsersService usersService;
    private readonly IMapper mapper;
    private readonly ILogger<UsersController> logger;

    public UsersController(IUsersService usersService, IMapper mapper, ILogger<UsersController> logger)
    {
        this.logger = logger; this.usersService = usersService; this.mapper = mapper;
    }

    [HttpPost]
    [Route(ApiRoutes.UsersRoutes.RegisterPart)]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserRequest request)
    {
        var createUserOperation =
            await usersService.RegisterUser(request.Email, request.Password, request.FirstName, request.LastName)
                .ConfigureAwait(false);

        return createUserOperation.IsFailure
            ? StatusCode(createUserOperation.ApiError.StatusCode, createUserOperation.ApiError.ToErrorDto())
            : Ok(mapper.Map<UserDto>(createUserOperation.Data));
    }

    [HttpPost]
    [Route(ApiRoutes.UsersRoutes.LoginPart)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var loginUser =
            await usersService.LoginUser(request.Email, request.Password)
                .ConfigureAwait(false);

        if (loginUser.IsFailure)
            return StatusCode(loginUser.ApiError.StatusCode, loginUser.ApiError.ToErrorDto());
        
        var response = new LoginResponse
        {
            User = mapper.Map<UserDto>(loginUser.Data),
            AccessToken = loginUser.Data.AccessToken!,
            RefreshToken = loginUser.Data.RefreshToken!
        };
        
        return Ok(response);
    }
    
    [HttpGet]
    [Route(ApiRoutes.UsersRoutes.GetUserInfoPart)] [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<UserDto>> GetUserInfo()
    {
        var userId = HttpContext.GetUserId(logger);
        if (userId is null)
            return Unauthorized();

        var getUserOperation =
            await usersService.GetUserInfo(userId.Value)
                .ConfigureAwait(false);

        return getUserOperation.IsFailure
            ? StatusCode(getUserOperation.ApiError.StatusCode, getUserOperation.ApiError.ToErrorDto())
            : Ok(mapper.Map<UserDto>(getUserOperation.Data));
    }
    
    [HttpPost]
    [Route(ApiRoutes.UsersRoutes.RefreshPart)]
    public async Task<ActionResult<RefreshTokenResult>> RefreshToken()
    {
        if (!HttpContext.Request.Headers.TryGetValue("RefreshToken", out var headerValue))
        {
            var error =  UsersApiErrors.RefreshTokenHeaderIsEmptyError();
            return StatusCode(error.StatusCode, error.ToErrorDto());
        }
        var extractedToken = JwtTokenHelper.ExtractToken(headerValue.ToString());
        var refreshToken =
            await usersService.RefreshToken(extractedToken)
                .ConfigureAwait(false);

        return refreshToken.IsSuccess
            ? Ok(new RefreshTokenResult { AccessToken = refreshToken.Data.accessToken, RefreshToken = refreshToken.Data.refreshToken })
            : StatusCode(refreshToken.ApiError.StatusCode, refreshToken.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.UsersRoutes.GetUserSchedulePart)] [AuthorizationFilter(UserRole.Student)]
    public async Task<ActionResult<List<Lesson>>> GetUserSchedule()
    {
        var userId = HttpContext.GetUserId(logger);
        if (userId is null)
            return Unauthorized();
        var getLessonsOperation = await usersService.GetUserSchedule(userId.Value).ConfigureAwait(false);
        return getLessonsOperation.IsFailure
            ? StatusCode(getLessonsOperation.ApiError.StatusCode, getLessonsOperation.ApiError.ToErrorDto())
            : Ok(getLessonsOperation.Data);
    }
}