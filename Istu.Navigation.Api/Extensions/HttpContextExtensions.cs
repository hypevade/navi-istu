using System.Security.Claims;

namespace Istu.Navigation.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid? GetUserId(this HttpContext httpContext, ILogger logger)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            logger.LogError("Unexpected error, userIdClaim is null, but it shouldn't be");
            return null;
        }
        return Guid.Parse(userIdClaim.Value);
    }
}