using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Users;

public class CreateUserRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
}