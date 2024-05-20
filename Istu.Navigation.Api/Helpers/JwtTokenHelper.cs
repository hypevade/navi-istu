namespace Istu.Navigation.Api.Helpers;

public static class JwtTokenHelper
{
    public static string? ExtractToken(string authorizationHeader)
    {
        if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authorizationHeader.Substring("Bearer ".Length).Trim();
        }

        if (string.IsNullOrEmpty(authorizationHeader))
            return null;
        return authorizationHeader;
    }
}