using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Istu.Navigation.Domain.Services;

public interface IJwtProvider
{
    public string GenerateToken(User user);
    public string GenerateToken(UserEntity user);
}

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions options = options.Value;

    public string GenerateToken(User user)
    {
        return GenerateToken(User.ToEntity(user));
    }

    public string GenerateToken(UserEntity user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));

        var signingCredentials = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(type: "id", value: user.Id.ToString()),
            new Claim(type: "email", value: user.Email),
            new Claim(type: "firstName", value: user.FirstName),
            new Claim(type: "lastName", value: user.LastName),
            new Claim(type: "role", value: user.Role.ToString())
        };

        var token = new JwtSecurityToken(signingCredentials: signingCredentials,
            expires: DateTime.Now.AddHours(options.ExpirationHours),
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}