using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IEdgesRepository : IRepository<EdgeEntity>
{
    public Task<List<EdgeEntity>> GetAllByBuildingId(Guid buildingId, int skip = 0, int take = 100);
    public Task<List<EdgeEntity>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100);
    public Task<List<EdgeEntity>> GetAllByBuildingObject(Guid buildingObjectId);
}

public class EdgesRepository : Repository<EdgeEntity>, IEdgesRepository
{
    public EdgesRepository(DbContext context) : base(context)
    { }

    public async Task<List<EdgeEntity>> GetAllByBuildingId(Guid buildingId, int skip = 0, int take = 100)
    {
        var edges = await DbSet.Where(edge => edge.BuildingId == buildingId)
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);
        return edges;
    }

    public async Task<List<EdgeEntity>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100)
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
    }
}