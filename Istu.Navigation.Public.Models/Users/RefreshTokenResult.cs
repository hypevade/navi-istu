namespace Istu.Navigation.Public.Models.Users;

public class RefreshTokenResult
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}