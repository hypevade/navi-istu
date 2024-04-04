using System.Linq.Expressions;
using Istu.Navigation.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Base;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(BuildingsDbContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
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

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync().ConfigureAwait(false);
    }
}