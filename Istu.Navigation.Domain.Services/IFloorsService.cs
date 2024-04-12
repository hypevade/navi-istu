using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services;

public interface IFloorsService
{
    public Task<OperationResult<List<Floor>>> GetAllByBuilding(Guid buildingId, int? minFloor = null, int? maxFloor = null); 
    public Task<OperationResult<Floor>> GetById(Guid buildingId, int floorNumber);
}

public class FloorsService : IFloorsService
{
    private readonly IBuildingObjectsService buildingObjectsService;
    private readonly IEdgesService edgesService;
    private readonly IImageService imageService;
    private readonly IBuildingsService buildingsService;

    public FloorsService(IBuildingObjectsService buildingObjectsService,
        IEdgesService edgesService,
        IImageService imageService,
        IBuildingsService buildingsService)
    {
        this.buildingObjectsService = buildingObjectsService;
        this.edgesService = edgesService;
        this.imageService = imageService;
        this.buildingsService = buildingsService;
    }

    public async Task<OperationResult<List<Floor>>> GetAllByBuilding(Guid buildingId, int? minFloor = null,
        int? maxFloor = null)
    {
        minFloor ??= 1;

        var building = await buildingsService
            .GetById(buildingId)
            .ConfigureAwait(false);
        if (building.IsFailure)
            return OperationResult<List<Floor>>.Failure(building.ApiError);

        maxFloor ??= building.Data.FloorNumbers;

        var floors = new List<Floor>();
        foreach (var number in Enumerable.Range(minFloor.Value, maxFloor.Value - minFloor.Value + 1))
        {
            var getFloor = await GetById(buildingId, number).ConfigureAwait(false);
            if (getFloor.IsFailure)
                return OperationResult<List<Floor>>.Failure(getFloor.ApiError);
            floors.Add(getFloor.Data);
        }

        return OperationResult<List<Floor>>.Success(floors);
    }

    public async Task<OperationResult<Floor>> GetById(Guid buildingId, int floorNumber)
    {
        var getBuildingObjects = await buildingObjectsService
            .GetAllByFilter(new BuildingObjectFilter { BuildingId = buildingId, Floor = floorNumber })
            .ConfigureAwait(false);
        if (getBuildingObjects.IsFailure)
            return OperationResult<Floor>.Failure(getBuildingObjects.ApiError);

        var getEdges = await edgesService
            .GetAllByFilter(new EdgeFilter { BuildingId = buildingId, FloorNumber = floorNumber })
            .ConfigureAwait(false);

        if (getEdges.IsFailure)
            return OperationResult<Floor>.Failure(getEdges.ApiError);

        var getImageLink = await imageService.GetByFloorId(buildingId, floorNumber).ConfigureAwait(false);
        if (getImageLink.IsFailure)
            return OperationResult<Floor>.Failure(getImageLink.ApiError);

        var floor = new Floor(buildingId, floorNumber, getBuildingObjects.Data, getEdges.Data, getImageLink.Data);
        return OperationResult<Floor>.Success(floor);
    }
}