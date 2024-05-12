using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;
//TODO: тестовый вариант 
[ApiController]
[Route("oauth")]
public class OAuthController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IHttpClientFactory httpClientFactory;

    public OAuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        this.configuration = configuration;
        this.httpClientFactory = httpClientFactory;
    }

    [HttpGet("authenticate")]
    public IActionResult AuthenticateUser()
    {
        var clientId = configuration["OAuth:ClientId"];
        var redirectUri = configuration["OAuth:RedirectUri"];
        var responseType = "code";
        var scope = "";
        var authorizationUrl =
            $"{configuration["OAuth:AuthorizationUrl"]}?response_type={responseType}&client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";
        
        return Redirect(authorizationUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> OAuthCallback(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Error: Authorization code is not provided.");
        }
        
        var token = await ExchangeCodeForTokenAsync(code);
        
        return Ok(token);
    }

    private async Task<string> ExchangeCodeForTokenAsync(string code)
    {
        var tokenUrl = configuration["OAuth:TokenUrl"];
        var clientId = configuration["OAuth:ClientId"];
        var clientSecret = configuration["OAuth:ClientSecret"];
        var redirectUri = configuration["OAuth:RedirectUri"];

        var client = httpClientFactory.CreateClient();
        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUri },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

        var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(requestBody));

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error retrieving access token.");
        }

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}