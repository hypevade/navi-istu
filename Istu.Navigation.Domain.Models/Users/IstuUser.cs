using Istu.Navigation.Domain.Models.Entities.User;

namespace Istu.Navigation.Domain.Models.Users;

public class IstuUser: User
{
    public IstuUser(Guid id, string email, string hashPassword, string firstName, string lastName, UserRole role,
        string istuId, string istuToken) : base(id, email, hashPassword, firstName, lastName, role)
    {
        IstuId = istuId;
        IstuToken = istuToken;
    }

    public string IstuId { get; set; }

    public string IstuToken { get; set; }
}