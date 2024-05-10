using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.IdentityModel.Tokens;

namespace Istu.Navigation.Domain.Services.Users;

public abstract class JwtTokenProvider
{
    private readonly TokenValidationParameters validationParameters;

    protected JwtTokenProvider(TokenValidationParameters validationParameters)
    {
        this.validationParameters = validationParameters;
    }

    public virtual string GenerateToken(UserEntity user)
    {
        var key = validationParameters.IssuerSigningKey;

        var signingCredentials = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(type: "id", value: user.Id.ToString()),
        };
        var token = new JwtSecurityToken(signingCredentials: signingCredentials,
            claims: claims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public virtual string GenerateToken(User user)
    {
       return GenerateToken(User.ToEntity(user)); 
    }
    
    public virtual OperationResult ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            
            var jwtToken = (JwtSecurityToken)validatedToken;
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                return OperationResult.Failure(UsersApiErrors.TokenExpiredError());
            }

            return OperationResult.Success();
        }
        catch (SecurityTokenValidationException stve)
        {
            Console.WriteLine($"Token validation failed: {stve.Message}");
            return OperationResult.Failure(UsersApiErrors.TokenIsNotValidError());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return OperationResult.Failure(CommonErrors.InternalServerError());
        }
    }
    
    public IEnumerable<Claim>? GetClaims(string tokenString)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
        return token.Claims;
    }
}