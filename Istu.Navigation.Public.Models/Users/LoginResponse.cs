namespace Istu.Navigation.Public.Models.Users;

public class LoginResponse
{
    public required UserDto User { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}