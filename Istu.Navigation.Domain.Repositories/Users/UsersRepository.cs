using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Users;


public interface IUsersRepository : IRepository<UserEntity>
{
    public Task<UserEntity?> GetByEmailAsync(string email);
    public Task UpdateRefreshTokenAsync(Guid userId, string refreshToken);
}
public class UsersRepository(AppDbContext context): Repository<UserEntity>(context), IUsersRepository 
{
    public Task<UserEntity?> GetByEmailAsync(string email)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Email == email);
    }
    public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var user = await DbSet.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
            return;

        user.RefreshToken = refreshToken;
        Context.Entry(user).State = EntityState.Modified;
        await Context.SaveChangesAsync();
    }
}