using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Domain.Repositories;

namespace Istu.Navigation.Infrastructure.Memory;

public class BuildingObjectsRepository : IBuildingObjectsRepository
{
    public Task<List<BuildingObject>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types)
    {
        throw new NotImplementedException();
    }

    public Task<List<BuildingObject>> GetAllByBuilding(Guid buildingId)
    {
        throw new NotImplementedException();
    }

    public Task<List<BuildingObject>> GetAllByFloor(Guid buildingId, int floor)
    {
        throw new NotImplementedException();
    }

    public Task<BuildingObject?> GetById(Guid buildingObjectId)
    {
        throw new NotImplementedException();
    }
}