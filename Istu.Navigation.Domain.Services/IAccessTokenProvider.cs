using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Istu.Navigation.Domain.Services;

public interface IAccessTokenProvider
{
    public string GenerateToken(User user);
    public string GenerateToken(UserEntity user);
}

public class AccessTokenProvider(IOptions<JwtOptions> options, TokenValidationParameters validationParameters)
    : JwtTokenProvider(validationParameters), IAccessTokenProvider
{
    private readonly JwtOptions options = options.Value;
    private readonly TokenValidationParameters validationParameters = validationParameters;

    public override string GenerateToken(User user)
    {
        return GenerateToken(User.ToEntity(user));
    }

    public override string GenerateToken(UserEntity user)
    {
        var key = validationParameters.IssuerSigningKey;

        var signingCredentials = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(type: "id", value: user.Id.ToString()),
            new Claim(type: "email", value: user.Email)
        };

        var token = new JwtSecurityToken(signingCredentials: signingCredentials,
            expires: DateTime.Now.AddHours(options.AccessTokenExpirationHours),
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}