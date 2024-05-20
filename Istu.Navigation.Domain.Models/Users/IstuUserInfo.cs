using Newtonsoft.Json;

namespace Istu.Navigation.Domain.Models.Users;

public class IstuUserInfo
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    [JsonProperty("last_name")]
    public string LastName { get; set; } = string.Empty;
    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;
}
