using System.Text;
using System.Text.Json;

namespace Istu.Navigation.TestClient;

public abstract class BaseClient
{
    protected readonly HttpClient Client;

    protected BaseClient(HttpClient client)
    {
        Client = client;
    }
    
    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
    {
        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await Client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(jsonResponse) ?? throw new InvalidOperationException();
    }

    public async Task<TResponse> GetAsync<TResponse>(string url)
    {
        var response = await Client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(jsonResponse) ?? throw new InvalidOperationException();
    }

    public async Task DeleteAsync(string url)
    {
        var response = await Client.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TResponse> PatchAsync<TRequest, TResponse>(string url, TRequest requestData)
    {
        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(jsonResponse) ?? throw new InvalidOperationException();
    }
}