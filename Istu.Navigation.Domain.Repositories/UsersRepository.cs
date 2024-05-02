using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;


public interface IUsersRepository : IRepository<UserEntity>
{
    public Task<UserEntity?> GetByEmailAsync(string email);
    public void UpdateRefreshToken(Guid userId, string refreshToken);
}
public class UsersRepository(DbContext context): Repository<UserEntity>(context), IUsersRepository 
{
    public Task<UserEntity?> GetByEmailAsync(string email)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Email == email);
    }

    public void UpdateRefreshToken(Guid userId, string refreshToken)
    {
        var user = DbSet.FirstOrDefault(x => x.Id == userId);
        if (user is null)
            return;
        user.RefreshToken = refreshToken;
        DbSet.Update(user);
    }
}