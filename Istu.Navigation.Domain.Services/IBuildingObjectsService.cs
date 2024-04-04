using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

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
    private readonly IBuildingsService buildingsService;

    public BuildingObjectsService(IBuildingObjectsRepository buildingObjectsRepository, IBuildingsService buildingsService)
    {
        this.buildingObjectsRepository = buildingObjectsRepository;
        this.buildingsService = buildingsService;
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
        var checkX = BuildingObject.CoordinateIsValid(buildingObject.X);
        var checkY = BuildingObject.CoordinateIsValid(buildingObject.Y);
        if (checkY || checkX)
            return OperationResult.Failure(BuildingsErrors.InvalidCoordinatesError(buildingObject.X, buildingObject.Y));
        if (string.IsNullOrWhiteSpace(buildingObject.Title))
            return OperationResult.Failure(BuildingsErrors.EmptyTitleError());

        var getBuilding = await buildingsService.GetBuildingById(buildingObject.BuildingId).ConfigureAwait(false);
        
        return getBuilding.IsFailure
            ? OperationResult.Failure(getBuilding.ApiError)
            : OperationResult.Success();
    }
}