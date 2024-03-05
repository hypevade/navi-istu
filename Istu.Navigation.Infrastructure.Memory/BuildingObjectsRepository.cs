using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Infrastructure.Memory;

public class BuildingObjectsRepository : IBuildingObjectsRepository
{
    public Task<OperationResult<List<BuildingObject>>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<List<BuildingObject>>> GetAllByBuilding(Guid buildingId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<List<BuildingObject>>> GetAllByFloor(Guid buildingId, int floor)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<BuildingObject>> GetById(Guid buildingObjectId)
    {
        throw new NotImplementedException();
    }
}