namespace Istu.Navigation.Public.Models.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    //public UserRole Role { get; set; }
}