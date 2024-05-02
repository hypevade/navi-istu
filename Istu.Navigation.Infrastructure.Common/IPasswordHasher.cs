namespace Istu.Navigation.Infrastructure.Common;

public interface IPasswordHasher
{
    public string Hash(string password);
    public bool Verify(string password, string hash);
}

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
}