using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF.Filters;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingsRepository : IRepository<BuildingEntity>
{
    public Task<List<BuildingEntity>> GetAllByFilterAsync(BuildingFilter filter);
}

public class BuildingsRepository : Repository<BuildingEntity> , IBuildingsRepository
{
    public BuildingsRepository(DbContext context) : base(context)
    {}
    
    public Task<List<BuildingEntity>> GetAllByFilterAsync(BuildingFilter filter)
    {
        var query = DbSet.AsQueryable();

        if (filter.BuildingId.HasValue)
        {
            query = query.Where(e => e.Id == filter.BuildingId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            query = query.Where(e=> e.Title == filter.Title);
        }

        if (filter.FloorNumber.HasValue)
        {
            query = query.Where(e => e.FloorNumbers == filter.FloorNumber.Value);
        }
        
        query = query.Skip(filter.Skip).Take(filter.Take);

        return query.ToListAsync();
    }
}