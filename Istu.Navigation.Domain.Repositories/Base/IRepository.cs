using System.Linq.Expressions;

namespace Istu.Navigation.Domain.Repositories.Base;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<List<TEntity>> GetAllAsync(int skip = 0, int take = 100);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> AddAsync(TEntity entity);
    Task<List<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    Task RemoveByIdAsync(Guid id);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task RemoveRangeAsync(IEnumerable<Guid> entityIds);
    public Task<int> SaveChangesAsync();
}