using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Istu.Navigation.Domain.Services.Users;

public interface IAccessTokenProvider
{
    public string GenerateToken(User user);
    public string GenerateToken(UserEntity user);
    public OperationResult<ClaimsPrincipal> ValidateToken(string token);
    public OperationResult<(Guid Id, UserRole Role)> GetUser(string token);
}

public class AccessTokenProvider : JwtTokenProvider, IAccessTokenProvider
{
    private readonly JwtOptions options;
    private readonly TokenValidationParameters validationParameters;

    public AccessTokenProvider(IOptions<JwtOptions> options, TokenValidationParameters validationParameters,
        ILogger<JwtTokenProvider> logger) : base(validationParameters, logger)
    {
        this.options = options.Value;
        this.validationParameters = validationParameters;
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
            new Claim(type: EmailClaim, value: user.Email),
            new Claim(type: RoleClaim, value: ((int)user.Role).ToString())
        };

        var token = new JwtSecurityToken(signingCredentials: signingCredentials,
            expires: DateTime.Now.AddHours(options.AccessTokenExpirationHours),
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public OperationResult<(Guid Id, UserRole Role)> GetUser(string token)
    {
        var principal = ValidateToken(token);
        if(principal.IsFailure)
            return OperationResult<(Guid Id, UserRole Role)>.Failure(principal.ApiError);
        var id = principal.Data.FindFirst(IdClaim);
        var role = principal.Data.FindFirst(RoleClaim);
        if (id is null || role is null)
        {
            Logger.LogError("Token was verified, but it was not possible to get the necessary stamps");
            return OperationResult<(Guid Id, UserRole Role)>.Failure(UsersApiErrors.TokenIsNotValidError());
        }
        
        return OperationResult<(Guid Id, UserRole Role)>.Success((Guid.Parse(id.Value), (UserRole)int.Parse(role.Value)));
    }
}