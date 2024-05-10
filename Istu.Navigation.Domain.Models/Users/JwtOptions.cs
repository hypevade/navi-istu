namespace Istu.Navigation.Domain.Models.Users;

public class JwtOptions
{
    public int AccessTokenExpirationHours { get; set; }
    
    public int RefreshTokenExpirationDays { get; set; }
}