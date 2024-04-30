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

    public async Task<List<TEntity>> GetAllAsync(int skip = 0, int take = 100)
    {
        return await DbSet.Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync().ConfigureAwait(false);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity).ConfigureAwait(false);
        return entity;
    }

    public async Task<List<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        var enumerable = entities.ToList();
        await DbSet.AddRangeAsync(enumerable).ConfigureAwait(false);
        return enumerable.ToList();
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

    public async Task RemoveRangeAsync(IEnumerable<Guid> entityIds)
    {
        var entities = new List<TEntity>();
        foreach (var entityId in entityIds)
        {
            var entity = await DbSet.FindAsync(entityId).ConfigureAwait(false);
            if (entity != null)
                entities.Add(entity);
        }
        
        DbSet.RemoveRange(entities);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync().ConfigureAwait(false);
    }
}