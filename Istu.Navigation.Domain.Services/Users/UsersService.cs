using AutoMapper;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Repositories.Users;
using Istu.Navigation.Infrastructure.Common;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.UsersApiErrors;

namespace Istu.Navigation.Domain.Services.Users;

public interface IUsersService
{
    Task<OperationResult<User>> RegisterUser(string email, string password, string firstName, string lastName);
    Task<OperationResult<User>> LoginUser(string email, string password);
    Task<OperationResult<(string accessToken, string refreshToken)>> RefreshToken(string refreshToken);
}

public class UsersService : IUsersService
{
    private readonly IPasswordHasher passwordHasher;
    private readonly IUsersRepository usersRepository;
    private readonly IMapper mapper;
    private readonly IAccessTokenProvider accessTokenProvider;
    private readonly IRefreshTokenProvider refreshTokenProvider;

    public UsersService(IPasswordHasher passwordHasher, IUsersRepository usersRepository, IMapper mapper,
        IAccessTokenProvider accessTokenProvider, IRefreshTokenProvider refreshTokenProvider)
    {
        this.passwordHasher = passwordHasher;
        this.usersRepository = usersRepository;
        this.mapper = mapper;
        this.accessTokenProvider = accessTokenProvider;
        this.refreshTokenProvider = refreshTokenProvider;
    }

    public async Task<OperationResult<User>> RegisterUser(string email, string password, string firstName,
        string lastName)
    {
        var checkUser = await CheckUser(email, password, firstName, lastName);
        if (checkUser.IsFailure)
            return OperationResult<User>.Failure(checkUser.ApiError);
        var hashPassword = passwordHasher.Hash(password);
        var userEntity = new UserEntity()
        {
            Id = Guid.NewGuid(),
            Email = email,
            HashPassword = hashPassword,
            FirstName = firstName,
            LastName = lastName,
            Role = UserRole.User
        };
        await usersRepository.AddAsync(userEntity);
        await usersRepository.SaveChangesAsync();
        return OperationResult<User>.Success(mapper.Map<User>(userEntity));
    }

    public async Task<OperationResult<User>> LoginUser(string email, string password)
    {
        var userEntity = await usersRepository.GetByEmailAsync(email);
        if (userEntity is null)
            return OperationResult<User>.Failure(UsersApiErrors.UserWithEmailNotFoundError(email));
        var verifyPassword = passwordHasher.Verify(password, userEntity.HashPassword);
        if (!verifyPassword)
            return OperationResult<User>.Failure(UsersApiErrors.IncorrectPasswordError(email));
        var accessToken = accessTokenProvider.GenerateToken(userEntity);
        var refreshToken = await UpdateRefreshToken(userEntity).ConfigureAwait(false);
        
        var user = mapper.Map<User>(userEntity);
        user.AccessToken = accessToken;
        user.RefreshToken = refreshToken;

        return OperationResult<User>.Success(user);
    }

    public async Task<OperationResult<(string accessToken, string refreshToken)>> RefreshToken(string refreshToken)
    {
        var validateToken = await ValidateRefreshTokenAndGetUser(refreshToken).ConfigureAwait(false);
        if (validateToken.IsFailure)
            return OperationResult<(string, string)>.Failure(validateToken.ApiError);
        var user = validateToken.Data;
        var accessToken = accessTokenProvider.GenerateToken(user);
        var newRefreshToken = await UpdateRefreshToken(user).ConfigureAwait(false); 

        return OperationResult<(string, string)>.Success((accessToken, newRefreshToken));
    }

    private async Task<string> UpdateRefreshToken(UserEntity user)
    {
        var newRefreshToken = refreshTokenProvider.GenerateToken(user);
        usersRepository.UpdateRefreshToken(user.Id, newRefreshToken);
        await usersRepository.SaveChangesAsync().ConfigureAwait(false);
        return newRefreshToken;
    }

    private async Task<OperationResult<UserEntity>> ValidateRefreshTokenAndGetUser(string refreshToken)
    {
        var validToken = refreshTokenProvider.ValidateToken(refreshToken);
        if (validToken.IsFailure)
            return OperationResult<UserEntity>.Failure(UsersApiErrors.TokenIsNotValidError());

        var userId = refreshTokenProvider.GetUserId(refreshToken);
        if (userId is null)
            return OperationResult<UserEntity>.Failure(UsersApiErrors.TokenIsNotValidError());

        var user = await usersRepository.GetByIdAsync(userId.Value);
        if (user is null)
            return OperationResult<UserEntity>.Failure(UsersApiErrors.TokenIsNotValidError());

        if (user.RefreshToken != refreshToken)
            return OperationResult<UserEntity>.Failure(UsersApiErrors.TokenIsNotValidError());

        return OperationResult<UserEntity>.Success(user);
    }

    //TODO: add validation
    private async Task<OperationResult> CheckUser(string email, string password, string firstName,
        string lastName)
    {
        return OperationResult.Success();
    }

}