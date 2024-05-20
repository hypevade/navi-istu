using Istu.Navigation.Domain.Models.Entities.User;

namespace Istu.Navigation.Domain.Models.Users;

public class IstuUser(
    Guid id,
    string email,
    string firstName,
    string lastName,
    string istuId,
    string istuAccessToken,
    string istuRefreshToken)
    : BaseUser(id, email, firstName, lastName, UserRole.Student)
{
    public string IstuId { get; set; } = istuId;
    public string IstuAccessToken { get; set; } = istuAccessToken;
    public string IstuRefreshToken { get; set; } = istuRefreshToken;
    

    public static IstuUser FromEntity(UserEntity userEntity)
    {
        return new IstuUser(userEntity.Id, userEntity.Email, userEntity.FirstName, userEntity.LastName,
            userEntity.IstuId ?? string.Empty, userEntity.IstuAccessToken ?? string.Empty,
            userEntity.IstuRefreshToken ?? string.Empty);
    }
}