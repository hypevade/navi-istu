using System.Net.Http.Headers;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Repositories.Users;
using Istu.Navigation.Domain.Services.Users;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Istu.Navigation.Domain.Services;

public interface IIstuService
{
    User GetUserInfo(Guid userId);
    Task<OperationResult<TokenResponse>> ExchangeCodeForTokenAsync(string code);
    OperationResult<string> GenerateRedirectUrl();

    Task<OperationResult<IstuUserInfo>> GetUserInfo(string istuAccessToken);

    Task<OperationResult<IstuUser>> RegisterIstuUser(IstuUserInfo userInfo, string istuAccessToken,
        string istuRefreshToken);
}

public class IstuService : IIstuService
{
    private readonly ILogger<IstuService> logger;
    private readonly OAuthOptions oAuthOptions;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IUsersRepository usersRepository;
    private readonly IAccessTokenProvider accessTokenProvider;
    private readonly IRefreshTokenProvider refreshTokenProvider;
    

    public IstuService(ILogger<IstuService> logger,
        IOptions<OAuthOptions> oAuthOptions,
        IHttpClientFactory httpClientFactory,
        IUsersRepository usersRepository,
        IAccessTokenProvider accessTokenProvider, IRefreshTokenProvider refreshTokenProvider)
    {
        this.logger = logger;
        this.oAuthOptions = oAuthOptions.Value;
        this.httpClientFactory = httpClientFactory;
        this.usersRepository = usersRepository;
        this.accessTokenProvider = accessTokenProvider;
        this.refreshTokenProvider = refreshTokenProvider;
    }

    public User GetUserInfo(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<IstuUserInfo>> GetUserInfo(string istuAccessToken)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", istuAccessToken);
        var response = await client.GetAsync(oAuthOptions.UserInfoUrl).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Error: Failed to get user info. Status code: {StatusCode}", response.StatusCode);
            return OperationResult<IstuUserInfo>.Failure(CommonErrors.InternalServerError());
        }
        
        IstuUserInfo? istuUserInfo;
        
        var content = await response.Content.ReadAsStringAsync();
        try
        {
            istuUserInfo = JsonConvert.DeserializeObject<IstuUserInfo>(content);
        }
        catch (Exception e)
        {
            logger.LogError("Error: Failed to deserialize user info. Message: {Message}", e.Message);
            throw;
        }

        if (istuUserInfo is null)
        {
            logger.LogError("Error: Failed to deserialize user info. Content: {Content}", content);
            return OperationResult<IstuUserInfo>.Failure(CommonErrors.InternalServerError());
        }
        return OperationResult<IstuUserInfo>.Success(istuUserInfo);
    }
    
    public async Task<OperationResult<IstuUser>> RegisterIstuUser(IstuUserInfo userInfo, string istuAccessToken, string istuRefreshToken)
    {
        var existedUser = await usersRepository.GetByEmailAsync(userInfo.Email).ConfigureAwait(false);
        if (existedUser is not null)
            return OperationResult<IstuUser>.Success(IstuUser.FromEntity(existedUser));
        
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = userInfo.Email,
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            Role = UserRole.Student,
            IstuId = userInfo.Id.ToString(),
            IstuAccessToken = istuAccessToken,
            IstuRefreshToken = istuRefreshToken
        };
        
        var accessToken = accessTokenProvider.GenerateToken(userEntity);
        var refreshToken = refreshTokenProvider.GenerateToken(userEntity);
        userEntity.RefreshToken = refreshToken;

        await usersRepository.AddAsync(userEntity).ConfigureAwait(false);
        await usersRepository.SaveChangesAsync().ConfigureAwait(false);

        var user = IstuUser.FromEntity(userEntity);

        user.AccessToken = accessToken;
        user.RefreshToken = refreshToken;
        
        return OperationResult<IstuUser>.Success(user);
    }

    public OperationResult<string> GenerateRedirectUrl()
    {
        var authorizationUrl =
            $"{oAuthOptions.AuthorizationUrl}?response_type={oAuthOptions.ResponseType}&client_id={oAuthOptions.ClientId}&redirect_uri={oAuthOptions.RedirectUri}";
        return OperationResult<string>.Success(authorizationUrl);
    }

    public async Task<OperationResult<TokenResponse>> ExchangeCodeForTokenAsync(string code)
    {
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