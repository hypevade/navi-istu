using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Infrastructure.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Istu.Navigation.Api.Controllers;
//TODO: тестовый вариант 
[ApiController]
[Route("oauth")]
public class OAuthController : ControllerBase
{
    private readonly OAuthOptions oAuthOptions;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<OAuthController> logger;

    public OAuthController(IOptions<OAuthOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<OAuthController> logger)
    {
        oAuthOptions = options.Value;
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    [HttpGet("authenticate")]
    public IActionResult AuthenticateUser()
    {
        if (string.IsNullOrEmpty(oAuthOptions.AuthorizationUrl) ||
            oAuthOptions.ClientId == default ||
            string.IsNullOrEmpty(oAuthOptions.RedirectUri) ||
            string.IsNullOrEmpty(oAuthOptions.ResponseType))
        {
            logger.LogError("Error: OAuth options are not provided.");
            return CommonErrors.InternalServerError().ToActionResult();
        }
        
        var authorizationUrl =
            $"{oAuthOptions.AuthorizationUrl}?response_type={oAuthOptions.ResponseType}&client_id={oAuthOptions.ClientId}&redirect_uri={oAuthOptions.RedirectUri}";
        
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

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }
        
        return Ok(token);
    }

    private async Task<string> ExchangeCodeForTokenAsync(string code)
    {
        if (string.IsNullOrEmpty(oAuthOptions.TokenUrl) || string.IsNullOrEmpty(oAuthOptions.RedirectUri) ||
            string.IsNullOrEmpty(oAuthOptions.ClientSecret))
        {
            return string.Empty;
        }
        
        var client = httpClientFactory.CreateClient();
        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", oAuthOptions.RedirectUri },
            { "client_id", oAuthOptions.ClientId.ToString() },
            { "client_secret", oAuthOptions.ClientSecret }
        };

        var response = await client.PostAsync(oAuthOptions.TokenUrl, new FormUrlEncodedContent(requestBody));

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error retrieving access token.");
        }

        var content = await response.Content.ReadAsStringAsync();
        logger.LogInformation(content);
        return content;
    }
}