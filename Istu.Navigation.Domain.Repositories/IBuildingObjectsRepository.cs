using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingObjectsRepository
{
    public Task<OperationResult<List<BuildingObject>>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types);
    public Task<OperationResult<List<BuildingObject>>> GetAllByBuilding(Guid buildingId);
    public Task<OperationResult<List<BuildingObject>>> GetAllByFloor(Guid buildingId, int floor);
    public Task<OperationResult<BuildingObject>> GetById(Guid buildingObjectId);
}