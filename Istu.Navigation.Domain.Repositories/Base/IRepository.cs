using System.Linq.Expressions;

namespace Istu.Navigation.Domain.Repositories.Base;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<List<TEntity>> GetAllAsync(int skip = 0, int take = 100);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> AddAsync(TEntity entity);
    Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(List<TEntity> entities);
    Task RemoveByIdAsync(Guid id);
    void Remove(TEntity entity);
    void RemoveRange(List<TEntity> entities);
    Task RemoveRangeAsync(List<Guid> entityIds);
    public Task<int> SaveChangesAsync();
}