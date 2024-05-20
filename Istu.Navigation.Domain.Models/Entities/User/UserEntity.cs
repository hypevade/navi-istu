using Istu.Navigation.Domain.Models.Users;

namespace Istu.Navigation.Domain.Models.Entities.User;

public class UserEntity : BaseEntity
{
    public required string Email { get; set; }
    public string? HashPassword { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required UserRole Role { get; set; }

    public string? RefreshToken { get; set; }
    
    public string? IstuId { get; set; }
    public string? IstuAccessToken { get; set; }
    public string? IstuRefreshToken { get; set; }
}