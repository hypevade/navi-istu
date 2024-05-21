using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Users;

public class LoginRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}