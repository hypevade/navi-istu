namespace Istu.Navigation.Domain.Models.Users;

public class OAuthOptions
{
    public int ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AuthorizationUrl { get; set; }
    public string? TokenUrl { get; set; }
    public string? RedirectUri { get; set; }
    public string? ResponseType { get; set; }
}