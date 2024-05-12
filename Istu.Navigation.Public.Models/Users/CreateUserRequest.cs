using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Users;

public class CreateUserRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    
    [Length(2, 20)]
    public required string FirstName { get; set; }
    
    [Length(2, 20)]
    public required string LastName { get; set; }
    
    [Length(6, 50)]
    public required string Password { get; set; }
}