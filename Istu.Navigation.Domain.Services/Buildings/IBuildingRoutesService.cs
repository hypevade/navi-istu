using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IBuildingRoutesService
{
    Task<OperationResult<BuildingRoute>> CreateRoute(Guid toId, Guid fromId = default);
}
public class BuildingRoutesService(
    IBuildingObjectsService objectService,
    IBuildingsService service,
    IRouteSearcher searcher,
    IFloorsBuilder floorsBuilder) : IBuildingRoutesService
{
    public async Task<OperationResult<BuildingRoute>> CreateRoute(Guid toId, Guid fromId = default)
    {
        //TODO: Добавить  поддержку, когда fromID = default
        var getOperation  = await GetTwoObjects(fromId, toId).ConfigureAwait(false);
        if (getOperation.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getOperation.ApiError);
        
        var toObject = getOperation.Data.toObject;
        var fromObject = getOperation.Data.fromObject;
        var buildingId = toObject.BuildingId;
        
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

        var floorRoutes = GetFloorRoutes(slicedRoute, getFloors.Data);

        var getBuilding = await service.GetByIdAsync(buildingId).ConfigureAwait(false);
        if (getBuilding.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getBuilding.ApiError);
        
        return OperationResult<BuildingRoute>.Success(new BuildingRoute(getBuilding.Data, floorRoutes));
    }

    private async Task<OperationResult<(BuildingObject fromObject, BuildingObject toObject)>> GetTwoObjects(Guid fromId, Guid toId)
    {
        var getToObject = await objectService.GetByIdAsync(toId).ConfigureAwait(false);
        if (getToObject.IsFailure)
            return OperationResult<(BuildingObject fromObject, BuildingObject toObject)>.Failure(getToObject.ApiError);

        var getFromObject = await objectService.GetByIdAsync(fromId).ConfigureAwait(false);
        if (getFromObject.IsFailure)
            return OperationResult<(BuildingObject fromObject, BuildingObject toObject)>.Failure(getFromObject.ApiError);
        
        return OperationResult<(BuildingObject fromObject, BuildingObject toObject)>.Success((getFromObject.Data, getToObject.Data));
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

    private List<FloorRoute> GetFloorRoutes(Dictionary<int, List<BuildingObject>> routes,
        List<Floor> floors)
    {
        var floorRoutes = new List<FloorRoute>();
        foreach (var floor in floors)
        {
            if (!routes.ContainsKey(floor.FloorNumber))
                throw new Exception($"Floor {floor.FloorNumber} not found in routes");
            
            floorRoutes.Add(new FloorRoute(floor, routes[floor.FloorNumber]));
        }

        return floorRoutes;
    }

    private (int start, int end) GetFloorsNumbers(BuildingObject fromBuildingObject, BuildingObject toBuildingObject)
    {
        var start = Math.Min(fromBuildingObject.Floor, toBuildingObject.Floor);
        var end = Math.Max(fromBuildingObject.Floor, toBuildingObject.Floor);
        return (start, end);
    }
}