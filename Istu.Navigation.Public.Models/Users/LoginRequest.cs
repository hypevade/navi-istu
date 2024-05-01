namespace Istu.Navigation.Public.Models.Users;

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}