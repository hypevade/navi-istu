using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Infrastructure.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Istu.Navigation.Domain.Services.Users;

public interface IRefreshTokenProvider
{
    public string GenerateToken(User user);
    public string GenerateToken(UserEntity user);
    public IEnumerable<Claim>? GetClaims(string token);
    public Guid? GetUserId(string token);
    public OperationResult<ClaimsPrincipal> ValidateToken(string token);
}

public class RefreshTokenProvider : JwtTokenProvider, IRefreshTokenProvider
{
    private readonly JwtOptions options;
    private readonly TokenValidationParameters validationParameters;

    public RefreshTokenProvider(IOptions<JwtOptions> options, TokenValidationParameters validationParameters, ILogger<RefreshTokenProvider> logger) : base(validationParameters, logger)
    {
        this.options = options.Value;
        this.validationParameters = validationParameters;
    }

    public Guid? GetUserId(string token)
    {
        var principal = ValidateToken(token);
        if (principal.IsFailure)
            return null;
        var id = principal.Data.FindFirst(IdClaim);
        return id is null 
            ? null 
            : Guid.Parse(id.Value);
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
            new Claim(type: IdClaim, value: user.Id.ToString()),
        };
        var token = new JwtSecurityToken(signingCredentials: signingCredentials,
            expires: DateTime.Now.AddDays(options.RefreshTokenExpirationDays),
            claims: claims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}