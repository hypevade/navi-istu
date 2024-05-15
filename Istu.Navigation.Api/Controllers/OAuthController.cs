using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Istu.Navigation.Api.Controllers;
//TODO: тестовый вариант, нужно будет переписать  
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
            return UsersApiErrors.CodeNotValidError().ToActionResult();
        
        var getTokenOperation = await ExchangeCodeForTokenAsync(code);

        if (getTokenOperation.IsFailure)
            return getTokenOperation.ApiError.ToActionResult();

        HttpContext.Response.Headers.Append("Authorization", $"Bearer {getTokenOperation.Data.AccessToken}");
        HttpContext.Response.Headers.Append("Refresh", $"Bearer {getTokenOperation.Data.RefreshToken}");
        return NoContent();
    }

    private async Task<OperationResult<TokenResponse>> ExchangeCodeForTokenAsync(string code)
    {
        if (string.IsNullOrEmpty(oAuthOptions.TokenUrl) || string.IsNullOrEmpty(oAuthOptions.RedirectUri) ||
            string.IsNullOrEmpty(oAuthOptions.ClientSecret))
        {
            logger.LogError("Error: OAuth options are not provided.");
            return OperationResult<TokenResponse>.Failure(CommonErrors.InternalServerError());
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
            logger.LogWarning($"Error: Failed to exchange code for token. Status code: {response.StatusCode}");
            return OperationResult<TokenResponse>.Failure(UsersApiErrors.CodeNotValidError());
        }
        
        TokenResponse? tokenResponse;

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
        }
        catch (Exception e)
        {
            logger.LogError("Error: Failed to deserialize token response. {Error}", e.Message);
            throw;
        }

        if (tokenResponse == null)
        {
            logger.LogError("Error: Failed to deserialize token response.");
            return OperationResult<TokenResponse>.Failure(CommonErrors.InternalServerError());
        }

        return OperationResult<TokenResponse>.Success(tokenResponse);
    }
}