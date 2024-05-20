namespace Istu.Navigation.Domain.Models.Users;

public abstract class BaseUser
{
    protected BaseUser(Guid id, string email, string firstName, string lastName, UserRole role)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserRole Role { get; set; }
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
}