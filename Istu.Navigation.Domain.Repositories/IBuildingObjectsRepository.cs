using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingObjectsRepository
{
    public Task<OperationResult<List<BuildingObject>>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types);
    public Task<OperationResult<List<BuildingObject>>> GetAllByBuilding(Guid buildingId, int startFloor = default, int endFloor = default);
    public Task<OperationResult<List<BuildingObject>>> GetAllByFloor(Guid buildingId, int floor);
    public Task<OperationResult<BuildingObject>> GetById(Guid buildingObjectId);
    public Task<OperationResult<BuildingObject>> GetDefaultInputByBuildingId(Guid buildingId);
}