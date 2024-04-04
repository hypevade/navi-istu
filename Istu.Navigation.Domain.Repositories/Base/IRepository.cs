using System.Linq.Expressions;

namespace Istu.Navigation.Domain.Repositories.Base;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync(int skip = 0, int take = 100);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    Task RemoveByIdAsync(Guid id);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    public Task<int> SaveChangesAsync();
}