using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services;

public interface IBuildingObjectsService
{
    public Task<OperationResult> Create(List<BuildingObject> buildingObjects);
    public Task<OperationResult<BuildingObject>> GetById(Guid buildingObjectId);
    public Task<OperationResult<List<BuildingObject>>> GetAllByBuildingId(Guid buildingId, int skip = 0, int take = 100); 
    public Task<OperationResult<List<BuildingObject>>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types, int skip = 0, int take = 100);
    public Task<OperationResult<List<BuildingObject>>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100);
    public Task<OperationResult<List<BuildingObject>>> GetAllByTypeInFloor(Guid buildingId, int floor, BuildingObjectType[] types, int skip = 0, int take = 100);
}

public class BuildingObjectsService : IBuildingObjectsService
{
    private readonly IBuildingObjectsRepository buildingObjectsRepository;
    private readonly IBuildingsRepository buildingsRepository;

    public BuildingObjectsService(IBuildingObjectsRepository buildingObjectsRepository, IBuildingsRepository buildingsRepository)
    {
        this.buildingObjectsRepository = buildingObjectsRepository;
        this.buildingsRepository = buildingsRepository;
    }

    public async Task<OperationResult> Create(List<BuildingObject> buildingObjects)
    {
        foreach (var bo in buildingObjects)
        {
            var check = await CheckCreateRequest(bo).ConfigureAwait(false);
            if(check.IsFailure)
                return check;
        }
        
        return await buildingObjectsRepository.Create(buildingObjects).ConfigureAwait(false);
    }

    public Task<OperationResult<BuildingObject>> GetById(Guid buildingObjectId)
    {
        return buildingObjectsRepository.GetById(buildingObjectId);
    }

    public Task<OperationResult<List<BuildingObject>>> GetAllByBuildingId(Guid buildingId, int skip = 0, int take = 100)
    {
        return buildingObjectsRepository.GetAllByBuilding(buildingId, skip, take);
    }

    public Task<OperationResult<List<BuildingObject>>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types, int skip = 0, int take = 100)
    {
        return buildingObjectsRepository.GetAllByTypeInBuilding(buildingId, types, skip, take);
    }

    public Task<OperationResult<List<BuildingObject>>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100)
    {
        return buildingObjectsRepository.GetAllByFloor(buildingId, floor, skip, take);
    }

    public Task<OperationResult<List<BuildingObject>>> GetAllByTypeInFloor(Guid buildingId, int floor, BuildingObjectType[] types, int skip = 0, int take = 100)
    {
        throw new NotImplementedException();
    }

    private async Task<OperationResult> CheckCreateRequest(BuildingObject buildingObject)
    {
        var getBuilding = await buildingsRepository.GetById(buildingObject.BuildingId).ConfigureAwait(false);
        return getBuilding.IsFailure
            ? OperationResult.Failure(getBuilding.ApiError)
            : OperationResult.Success();
    }
}