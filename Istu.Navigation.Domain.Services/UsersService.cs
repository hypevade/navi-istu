using AutoMapper;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Common;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.UsersApiErrors;

namespace Istu.Navigation.Domain.Services;

public interface IUsersService
{
    Task<OperationResult<User>> RegisterUser(string email, string password, string firstName, string lastName);
    Task<OperationResult<string>> LoginUser(string email, string password);
}
public class UsersService : IUsersService
{
    private readonly IPasswordHasher passwordHasher;
    private readonly IUsersRepository usersRepository;
    private readonly IMapper mapper;
    private readonly IJwtProvider jwtProvider;

    public UsersService(IPasswordHasher passwordHasher, IUsersRepository usersRepository, IMapper mapper,
        IJwtProvider jwtProvider)
    {
        this.passwordHasher = passwordHasher;
        this.usersRepository = usersRepository;
        this.mapper = mapper;
        this.jwtProvider = jwtProvider;
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
    
    public async Task<OperationResult<string>> LoginUser(string email, string password)
    {
        var user = await usersRepository.GetByEmailAsync(email);
        if(user is null)
            return OperationResult<string>.Failure(UsersApiErrors.UserWithEmailNotFoundError(email));
        var verifyPassword = passwordHasher.Verify(password, user.HashPassword);
        if(!verifyPassword)
            return OperationResult<string>.Failure(UsersApiErrors.IncorrectPasswordError(email));
        var token = jwtProvider.GenerateToken(user);
        return OperationResult<string>.Success(token);
    }

    private async Task<OperationResult> CheckUser(string email, string password, string firstName,
        string lastName)
    {
        return OperationResult.Success();
    }

}