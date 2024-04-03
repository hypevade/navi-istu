using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingObjectsRepository
{
    public Task<OperationResult<List<BuildingObject>>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types, int skip = 0, int take = 100);
    public Task<OperationResult<List<BuildingObject>>> GetAllByBuilding(Guid buildingId, int skip = 0, int take = 100);
    public Task<OperationResult<List<BuildingObject>>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100);
    public Task<OperationResult<BuildingObject>> GetById(Guid buildingObjectId);
    public Task<OperationResult<BuildingObject>> GetDefaultInputByBuildingId(Guid buildingId);
    public Task<OperationResult> Create(BuildingObject buildingObject);
    public Task<OperationResult> Create(List<BuildingObject> buildingObjects);
}