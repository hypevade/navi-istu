namespace Istu.Navigation.Domain.Models.Users;

public class OAuthOptions
{
    public int ClientId { get; set; }
    public string ClientSecret { get; set; } = string.Empty;
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string UserInfoUrl { get; set; } = string.Empty;
    public string ResponseType { get; set; } = string.Empty;
}