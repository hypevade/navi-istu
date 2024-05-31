using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.EF.Filters;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Buildings;

public interface IBuildingObjectsRepository : IRepository<BuildingObjectEntity>
{
    public Task<List<BuildingObjectEntity>> GetAllByFilterAsync(BuildingObjectFilter filter);
}

public class BuildingObjectsRepository(AppDbContext context)
    : Repository<BuildingObjectEntity>(context), IBuildingObjectsRepository
{
    public Task<List<BuildingObjectEntity>> GetAllByFilterAsync(BuildingObjectFilter filter)
    {
        var query = DbSet.AsQueryable();

        if (filter.BuildingId.HasValue)
            query = query.Where(e => e.BuildingId == filter.BuildingId.Value);

        if (filter.BuildingObjectId.HasValue)
            query = query.Where(e => e.Id == filter.BuildingObjectId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(e => e.Title == filter.Title);

        if (filter.Floor.HasValue)
            query = query.Where(e => e.Floor == filter.Floor.Value);

        if (filter.Type.HasValue)
            query = query.Where(e => e.Type == filter.Type.Value);
            

        query = query.Skip(filter.Skip).Take(filter.Take);

        return query.ToListAsync();
    }
}