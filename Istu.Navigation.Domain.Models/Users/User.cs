using Istu.Navigation.Domain.Models.Entities.User;

namespace Istu.Navigation.Domain.Models.Users;

public class User: BaseUser
{
    public User(Guid id, string email, string hashPassword, string firstName, string lastName, UserRole role) : base(id, email, firstName, lastName, role)
    {
        Id = id;
        Email = email;
        HashPassword = hashPassword;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }
    
    public string HashPassword { get; set; }

    public static UserEntity ToEntity(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        HashPassword = user.HashPassword,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Role = user.Role,
        RefreshToken = user.RefreshToken
    };
}