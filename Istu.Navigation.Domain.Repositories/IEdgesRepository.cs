using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF.Filters;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IEdgesRepository : IRepository<EdgeEntity>
{
    public Task<List<EdgeEntity>> GetAllByFilterAsync(EdgeFilter filter);
}

public class EdgesRepository : Repository<EdgeEntity>, IEdgesRepository
{
    public EdgesRepository(DbContext context) : base(context)
    { }

    /*public async Task<List<EdgeEntity>> GetAllByBuildingId(Guid buildingId, int skip = 0, int take = 100)
    {
        var edges = await DbSet.Where(edge => edge.BuildingId == buildingId)
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);
        return edges;
    }*/
    
    public async Task<List<EdgeEntity>> GetAllByFilterAsync(EdgeFilter filter)
    {
        var query = DbSet.AsQueryable();

        if (filter.BuildingId.HasValue)
        {
            query = query.Where(e => e.BuildingId == filter.BuildingId.Value);
        }

        if (filter.BuildingObjectId.HasValue)
        {
            query = query.Where(e =>
                e.FromObject == filter.BuildingObjectId.Value || e.ToObject == filter.BuildingObjectId.Value);
        }

        if (filter.FloorNumber.HasValue)
        {
            query = query.Where(e => e.FloorNumber == filter.FloorNumber.Value);
        }
        
        query = query.Skip(filter.Skip).Take(filter.Take);

        return await query.ToListAsync();
    }

    /*public async Task<List<EdgeEntity>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100)
    {
        var edges = await DbSet.Where(x => x.BuildingId == buildingId && x.FloorNumber == floor)
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);
        return edges;
    }

    public async Task<List<EdgeEntity>> GetAllByBuildingObject(Guid buildingObjectId)
    {
        var edges = await DbSet
            .Where(x => x.FromObject == buildingObjectId || x.ToObject == buildingObjectId)
            .ToListAsync()
            .ConfigureAwait(false);
        return edges;
    }*/
}