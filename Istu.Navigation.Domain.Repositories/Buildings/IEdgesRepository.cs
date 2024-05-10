using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF.Filters;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Buildings;

public interface IEdgesRepository : IRepository<EdgeEntity>
{
    public Task<List<EdgeEntity>> GetAllByFilterAsync(EdgeFilter filter);
}

public class EdgesRepository : Repository<EdgeEntity>, IEdgesRepository
{
    public EdgesRepository(DbContext context) : base(context)
    { }
    
    public Task<List<EdgeEntity>> GetAllByFilterAsync(EdgeFilter filter)
    {
        var query = DbSet.AsQueryable();

        if (filter.BuildingId.HasValue)
        {
            query = query.Where(e => e.BuildingId == filter.BuildingId.Value);
        }

        if (filter.Floor.HasValue)
        {
            query = query.Where(e => e.FloorNumber == filter.Floor.Value);
        }

        switch (filter)
        {
            case { FromBuildingObjectId: not null, ToBuildingObjectId: not null }:
                query = query.Where(e =>
                    e.FromObject == filter.FromBuildingObjectId && e.ToObject == filter.ToBuildingObjectId
                    || e.FromObject == filter.ToBuildingObjectId && e.ToObject == filter.FromBuildingObjectId);
                break;
            
            case { FromBuildingObjectId: not null }:
                query = query.Where(e => e.FromObject == filter.FromBuildingObjectId || e.ToObject == filter.FromBuildingObjectId);
                break;
            
            case { ToBuildingObjectId: not null }:
                query = query.Where(e => e.ToObject == filter.ToBuildingObjectId || e.FromObject == filter.ToBuildingObjectId);
                break;
        }

        query = query.Skip(filter.Skip).Take(filter.Take);

        return query.ToListAsync();
    }
}