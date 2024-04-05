using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingObjectsRepository : IRepository<BuildingObjectEntity>
{
    public Task<IEnumerable<BuildingObjectEntity>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types, int skip = 0, int take = 100);
    public Task<IEnumerable<BuildingObjectEntity>> GetAllByBuilding(Guid buildingId, int skip = 0, int take = 100);
    public Task<IEnumerable<BuildingObjectEntity>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100);
}

public class BuildingObjectsRepository : Repository<BuildingObjectEntity>, IBuildingObjectsRepository
{
    public BuildingObjectsRepository(DbContext context) : base(context)
    {}

    public async Task<IEnumerable<BuildingObjectEntity>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types, int skip = 0, int take = 100)
    {
        var objects = await DbSet.Where(x => x.BuildingId == buildingId && types.Contains(x.Type))
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);
        
        return objects;
    }

    public async Task<IEnumerable<BuildingObjectEntity>> GetAllByBuilding(Guid buildingId, int skip = 0, int take = 100)
    {
        var objects = await DbSet.Where(x => x.BuildingId == buildingId)
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);
        
        return objects;
    }

    public async Task<IEnumerable<BuildingObjectEntity>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100)
    {
        var objects = await DbSet.Where(x => x.BuildingId == buildingId && x.Floor == floor)
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);

        return objects; 
    }
}