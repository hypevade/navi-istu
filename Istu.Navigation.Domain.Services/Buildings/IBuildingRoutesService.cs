using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IBuildingRoutesService
{
    Task<OperationResult<BuildingRoute>> CreateRoute(Guid buildingId, Guid toId, Guid fromId = default);
    //Task<OperationResult<BuildingRoute>> GetRouteById(Guid routeId);
}
public class BuildingRoutesService(
    IBuildingObjectsService objectService,
    IBuildingsService service,
    IRouteSearcher searcher,
    IFloorsBuilder floorsBuilder) : IBuildingRoutesService
{
    public async Task<OperationResult<BuildingRoute>> CreateRoute(Guid buildingId, Guid toId, Guid fromId = default)
    {
        //TODO: Добавить  поддержку, когда fromID = default

        var getToObject = await objectService.GetById(toId).ConfigureAwait(false);
        if (getToObject.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getToObject.ApiError);

        var getFromObject = await objectService.GetById(fromId).ConfigureAwait(false);
        if (getFromObject.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFromObject.ApiError);

        var toObject = getToObject.Data;
        var fromObject = getFromObject.Data;

        var (start, end) = GetFloorsNumbers(fromObject, toObject);
        var getFloors = await floorsBuilder.GetFloorsByBuilding(buildingId, start, end).ConfigureAwait(false);
        if (getFloors.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFloors.ApiError);

        var objects = getFloors.Data.SelectMany(x => x.Objects).ToList();
        var edges = getFloors.Data.SelectMany(x => x.Edges).ToList();

        var getFullRoute = searcher.CreateRoute(fromObject, toObject, objects, edges);
        if (getFullRoute.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFullRoute.ApiError);

        var fullRoute = getFullRoute.Data;
        var slicedRoute = SliceRouteByFloors(fullRoute);

        var getFloorRoutes = GetFloorRoutes(slicedRoute, getFloors.Data);
        if (getFloorRoutes.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFloorRoutes.ApiError);

        var getBuilding = await service.GetByIdAsync(buildingId).ConfigureAwait(false);
        if (getBuilding.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getBuilding.ApiError);

        var resultRoute = new BuildingRoute(getBuilding.Data,
            getFloorRoutes.Data,
            startObject: fromObject,
            finishObject: toObject);

        return OperationResult<BuildingRoute>.Success(resultRoute);
    }

    private Dictionary<int, List<BuildingObject>> SliceRouteByFloors(List<BuildingObject> totalPath)
    {
        var result = new Dictionary<int, List<BuildingObject>>();
        if (!totalPath.Any())
            return result;

        foreach (var obj in totalPath)
        {
            var floorNumber = obj.Floor;
            if (result.ContainsKey(obj.Floor))
                result[floorNumber].Add(obj);
            else
                result.Add(floorNumber, [obj]);
        }

        return result;
    }

    private OperationResult<List<FloorRoute>> GetFloorRoutes(Dictionary<int, List<BuildingObject>> routes,
        List<Floor> floors)
    {
        var floorRoutes = new List<FloorRoute>();
        foreach (var floor in floors)
        {
            if (!routes.ContainsKey(floor.FloorNumber))
                return OperationResult<List<FloorRoute>>.Failure(CommonErrors.InternalServerError());

            var route = routes[floor.FloorNumber];
            floorRoutes.Add(new FloorRoute(route, floor, route[0], route[^1]));
        }

        return OperationResult<List<FloorRoute>>.Success(floorRoutes);
    }

    private (int start, int end) GetFloorsNumbers(BuildingObject fromBuildingObject, BuildingObject toBuildingObject)
    {
        var start = Math.Min(fromBuildingObject.Floor, toBuildingObject.Floor);
        var end = Math.Max(fromBuildingObject.Floor, toBuildingObject.Floor);
        return (start, end);
    }
}