using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Infrastructure.Errors;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Istu.Navigation.Domain.Services.Users;

public interface IRefreshTokenProvider
{
    public string GenerateToken(User user);
    public string GenerateToken(UserEntity user);
    public IEnumerable<Claim>? GetClaims(string token);
    public Guid? GetUserId(string token);
    public OperationResult ValidateToken(string token);
}

public class RefreshTokenProvider(IOptions<JwtOptions> options, TokenValidationParameters validationParameters)
    : JwtTokenProvider(validationParameters), IRefreshTokenProvider
{
    private readonly JwtOptions options = options.Value;
    private readonly TokenValidationParameters validationParameters = validationParameters;

    public Guid? GetUserId(string token)
    {
        var claims = GetClaims(token);
        if (claims is null)
            return null;
        return Guid.Parse(claims.First(x => x.Type == "id").Value);
    }

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
        };
        var token = new JwtSecurityToken(signingCredentials: signingCredentials,
            expires: DateTime.Now.AddDays(options.RefreshTokenExpirationDays),
            claims: claims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}