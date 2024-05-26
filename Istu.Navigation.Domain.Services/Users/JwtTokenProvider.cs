using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Istu.Navigation.Domain.Services.Users;

public abstract class JwtTokenProvider
{
    private readonly TokenValidationParameters validationParameters;
    protected readonly ILogger Logger;
    protected const string IdClaim = "id";
    protected const string EmailClaim = "email";
    protected const string RoleClaim = "user-role";

    protected JwtTokenProvider(TokenValidationParameters validationParameters, ILogger logger)
    {
        this.validationParameters = validationParameters;
        Logger = logger;
    }

    public virtual string GenerateToken(UserEntity user)
    {
        var key = validationParameters.IssuerSigningKey;

        var signingCredentials = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(type: IdClaim, value: user.Id.ToString()),
        };
        var token = new JwtSecurityToken(signingCredentials: signingCredentials,
            claims: claims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public virtual string GenerateToken(User user)
    {
       return GenerateToken(User.ToEntity(user)); 
    }
    
    public virtual OperationResult<ClaimsPrincipal> ValidateToken(string token)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(token))
                return OperationResult<ClaimsPrincipal>.Failure(UsersApiErrors.TokenIsNotValidError());
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken ar);

            return OperationResult<ClaimsPrincipal>.Success(principal);
        }
        catch (SecurityTokenExpiredException ste)
        {
            Logger.LogWarning("Token expired: {ste}", ste);
            return OperationResult<ClaimsPrincipal>.Failure(UsersApiErrors.TokenExpiredError());
        }
        catch (SecurityTokenValidationException stve)
        {
            Logger.LogWarning($"Token validation failed: {stve.Message}");
            return OperationResult<ClaimsPrincipal>.Failure(UsersApiErrors.TokenIsNotValidError());
        }
        catch (SecurityTokenMalformedException stme)
        {
            Logger.LogWarning("Invalid token: {stme}", stme);
            return OperationResult<ClaimsPrincipal>.Failure(UsersApiErrors.TokenIsNotValidError());
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"Unexpected error: Token validation failed: {ex.Message}");
            return OperationResult<ClaimsPrincipal>.Failure(CommonErrors.InternalServerError());
        }
    }
    
    public IEnumerable<Claim>? GetClaims(string tokenString)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
        return token.Claims;
    }
}