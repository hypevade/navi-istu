using Istu.Navigation.Domain.Models.Entities.User;

namespace Istu.Navigation.Domain.Models.Users;

public class User
{
    public User(Guid id, string email, string hashPassword, string firstName, string lastName, UserRole role)
    {
        Id = id;
        Email = email;
        HashPassword = hashPassword;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string HashPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserRole Role { get; set; }

    public static UserEntity ToEntity(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        HashPassword = user.HashPassword,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Role = user.Role
    };
}