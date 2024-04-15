using System.Text;
using System.Text.Json;
using Istu.Navigation.Infrastructure.Errors;
using Microsoft.AspNetCore.WebUtilities;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public abstract class BaseClient
{
    protected readonly HttpClient Client;

    protected BaseClient(HttpClient client)
    {
        Client = client;
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
    {
        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await Client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(jsonResponse) ?? throw new InvalidOperationException();
    }
    protected async Task<OperationResult<TResponse>> PostAsync1<TRequest, TResponse>(string url, TRequest requestData)
    {
        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await Client.PostAsync(url, content);
        return await GetResponse<TResponse>(response).ConfigureAwait(false);
    }

    protected async Task<TResponse> GetAsync<TResponse>(string url, Dictionary<string, string?>? queries = null)
    {
        if(queries != null && queries.Any())
            url = QueryHelpers.AddQueryString(url, queries);
        
        var response = await Client.GetAsync(url);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<TResponse>(jsonResponse) ?? throw new InvalidOperationException();
    }
    
    protected async Task<OperationResult<TResponse>> GetAsync1<TResponse>(string url, Dictionary<string, string?>? queries = null)
    {
        if (queries != null && queries.Any())
            url = QueryHelpers.AddQueryString(url, queries);
    
        var response = await Client.GetAsync(url);
        return await GetResponse<TResponse>(response);
    }

    protected async Task<OperationResult<TResponse>> GetResponse<TResponse>(HttpResponseMessage httpResponseMessage)
    {
        var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        // Проверяем, успешен ли статус HTTP ответа
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var data = JsonSerializer.Deserialize<TResponse>(jsonResponse) ??
                       throw new InvalidOperationException("Не удалось десериализовать ответ.");
            return OperationResult<TResponse>.Success(data);
        }

        // Создаем объект ошибки, используя информацию из HTTP-ответа
        var apiError = JsonSerializer.Deserialize<ApiError>(jsonResponse) ??
                       throw new InvalidOperationException("Не удалось десериализовать ответ.");
        return OperationResult<TResponse>.Failure(apiError);
    }

    protected async Task DeleteAsync(string url)
    {
        var response = await Client.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
    protected async Task<OperationResult> DeleteAsync1(string url)
    {
        var response = await Client.DeleteAsync(url);
        if (response.IsSuccessStatusCode)
            return OperationResult.Success();
        
        var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var apiError = JsonSerializer.Deserialize<ApiError>(jsonResponse) ??
                       throw new InvalidOperationException("Не удалось десериализовать ответ.");
        return OperationResult.Failure(apiError);
    }

    protected async Task<TResponse> PatchAsync<TRequest, TResponse>(string url, TRequest requestData)
    {
        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(jsonResponse) ?? throw new InvalidOperationException();
    }

    protected async Task PatchAsync<TRequest>(string url, TRequest requestData)
    {
        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}