using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IFloorsBuilder
{
    public Task<OperationResult<List<Floor>>> GetFloorsByBuilding(Guid buildingId, int minFloor = 1,
        int maxFloor = int.MaxValue); 
    public Task<OperationResult<Floor>> GetFloor(Guid buildingId, int floorNumber);
}

public class FloorsBuilder : IFloorsBuilder
{
    private readonly IBuildingObjectsService buildingObjectsService;
    private readonly IFloorsRepository floorsRepository;
    private readonly IEdgesService edgesService;
    private readonly IImageService imageService;

    public FloorsBuilder(IBuildingObjectsService buildingObjectsService,
        IFloorsRepository floorsRepository,
        IEdgesService edgesService,
        IImageService imageService)
    {
        this.buildingObjectsService = buildingObjectsService;
        this.floorsRepository = floorsRepository;
        this.edgesService = edgesService;
        this.imageService = imageService;
    }

    public async Task<OperationResult<List<Floor>>> GetFloorsByBuilding(Guid buildingId, int minFloor = 1,
        int maxFloor = int.MaxValue)
    {
        if (minFloor < 1) minFloor = 1;
        if (minFloor > maxFloor)
            return OperationResult<List<Floor>>.Failure(
                BuildingsApiErrors.MinFloorGreaterThanMaxFloorError(minFloor, maxFloor));

        var floorsEntities = await floorsRepository.GetAllByBuildingIdAsync(buildingId, minFloor, maxFloor)
            .ConfigureAwait(false);
        var sortedFloors = floorsEntities.OrderBy(x => x.FloorNumber);

        var floors = new List<Floor>();
        foreach (var floorEntity in sortedFloors)
        {
            var floor = await BuildFloor(floorEntity).ConfigureAwait(false);
            if (floor.IsFailure)
            {
                return OperationResult<List<Floor>>.Failure(floor.ApiError);
            }

            floors.Add(floor.Data);
        }

        return OperationResult<List<Floor>>.Success(floors);
    }

    public async Task<OperationResult<Floor>> GetFloor(Guid buildingId, int floorNumber)
    {
        var floorEntity = await floorsRepository.GetByBuildingIdAsync(buildingId, floorNumber).ConfigureAwait(false);
        if (floorEntity is null)
            return OperationResult<Floor>.Failure(
                BuildingsApiErrors.FloorWithBuildingAndFloorNumberNotFoundError(buildingId, floorNumber));
        return await BuildFloor(floorEntity).ConfigureAwait(false);
    }

    private async Task<OperationResult<Floor>> BuildFloor(FloorEntity floorEntity)
    {
        var buildingId = floorEntity.BuildingId;
        var floorNumber = floorEntity.FloorNumber;
        var buildingObjectFilter = new BuildingObjectFilter
        {
            BuildingId = buildingId,
            Floor = floorNumber
        };
        var objects = await buildingObjectsService.GetAllByFilter(buildingObjectFilter).ConfigureAwait(false);

        if (objects.IsFailure)
            return OperationResult<Floor>.Failure(objects.ApiError);

        var edgeFilter = new EdgeFilter
        {
            BuildingId = buildingId,
            Floor = floorNumber
        };
        var edges = await edgesService.GetAllByFilter(edgeFilter).ConfigureAwait(false);

        if (edges.IsFailure)
            return OperationResult<Floor>.Failure(edges.ApiError);

        var images = await imageService.GetInfosByObjectIdAsync(floorEntity.Id).ConfigureAwait(false);
        if (images.IsFailure)
            return OperationResult<Floor>.Failure(images.ApiError);

        return OperationResult<Floor>.Success(new Floor(buildingId, floorNumber, objects.Data, edges.Data));
    }
}