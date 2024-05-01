namespace Istu.Navigation.Domain.Models.Users;

public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationHours { get; set; }
}