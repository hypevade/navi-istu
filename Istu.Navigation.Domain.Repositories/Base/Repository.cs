using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Base;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbContext context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(DbContext context)
    {
        this.context = context;
        DbSet = this.context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id).ConfigureAwait(false);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(int skip = 0, int take = 100)
    {
        return await DbSet.Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync().ConfigureAwait(false);
    }

    public async Task AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity).ConfigureAwait(false);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await DbSet.AddRangeAsync(entities).ConfigureAwait(false);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }
    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }
    
    public async Task RemoveByIdAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity != null)
            DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync().ConfigureAwait(false);
    }
}