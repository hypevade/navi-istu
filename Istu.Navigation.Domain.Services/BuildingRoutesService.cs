using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors;

namespace Istu.Navigation.Domain.Services;

public class BuildingRoutesService(
    IBuildingObjectsService buildingObjectService,
    IBuildingsService buildingsService,
    IFloorsService floorsService,
    IRouteSearcher routeSearcher): IBuildingRoutesService
{
    private readonly IBuildingObjectsService buildingObjectService = buildingObjectService;
    private readonly IFloorsService floorsService = floorsService;
    private readonly IRouteSearcher routeSearcher = routeSearcher;
    private readonly IBuildingsService buildingsService = buildingsService;

    public async Task<OperationResult<BuildingRoute>> CreateRoute(Guid buildingId, Guid toId, Guid fromId = default)
    {
        //TODO: Добавить  поддержку, когда fromID = default
        
        var getToObject = await buildingObjectService.GetById(toId).ConfigureAwait(false);
        if (getToObject.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getToObject.ApiError);
        
        var getFromObject = await buildingObjectService.GetById(fromId).ConfigureAwait(false);
        if (getFromObject.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFromObject.ApiError);

        var toObject = getToObject.Data;
        var fromObject = getFromObject.Data;

        var floorNumbers = GetFloorsNumbers(fromObject, toObject);

        var getFloors = await GetFloors(buildingId, floorNumbers).ConfigureAwait(false);
        if (getFloors.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFloors.ApiError);

        var objects = getFloors.Data.SelectMany(x => x.Objects).ToList();
        var edges = getFloors.Data.SelectMany(x => x.Edges).ToList();

        var getFullRoute = routeSearcher.CreateRoute(fromObject, toObject, objects, edges);
        if(getFullRoute.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFullRoute.ApiError);
        
        var fullRoute = getFullRoute.Data;
        var slicedRoute = SliceRouteByFloors(fullRoute);

        var getFloorRoutes = GetFloorRoutes(slicedRoute, getFloors.Data);
        if (getFloorRoutes.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFloorRoutes.ApiError);

        var getBuilding = await buildingsService.GetBuildingById(buildingId).ConfigureAwait(false);
        if(getBuilding.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getBuilding.ApiError);;

        var resultRoute = new BuildingRoute(getBuilding.Data,
            getFloorRoutes.Data,
            startObject: fromObject,
            finishObject: toObject);
        
        return OperationResult<BuildingRoute>.Success(resultRoute);
    }
    
    /*public async Task<OperationResult<BuildingRoute>> GetRouteById(Guid routeId)
    {
        var route = await routeRepository.GetById(routeId).ConfigureAwait(false);
        return route;
    }*/

    private Dictionary<int, List<BuildingObject>> SliceRouteByFloors(List<BuildingObject> totalPath)
    {
        var result = new Dictionary<int, List<BuildingObject>>();
        if (!totalPath.Any())
            return result;
        
        foreach (var obj in totalPath)
        {
            var floorNumber = obj.FloorNumber;
            if (result.ContainsKey(obj.FloorNumber))
                result[floorNumber].Add(obj);
            else
                result.Add(floorNumber, [obj]);
        }

        return result;
    }

    private OperationResult<List<FloorRoute>> GetFloorRoutes(Dictionary<int, List<BuildingObject>> routes, List<Floor> floors)
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


    private async Task<OperationResult<List<Floor>>> GetFloors(Guid buildingId, IEnumerable<int> numbersOfFloors)
    {
        var floors = new List<Floor>();
        foreach (var floorNumber in numbersOfFloors)
        {
            var getFloor = await floorsService.GetById(buildingId, floorNumber).ConfigureAwait(false);
            if (getFloor.IsFailure)
                return OperationResult<List<Floor>>.Failure(getFloor.ApiError);

            floors.Add(getFloor.Data);
        }

        return OperationResult<List<Floor>>.Success(floors);
    }

    private IEnumerable<int> GetFloorsNumbers(BuildingObject fromBuildingObject, BuildingObject toBuildingObject)
    {
        var start = Math.Min(fromBuildingObject.FloorNumber, toBuildingObject.FloorNumber);
        var end = Math.Max(fromBuildingObject.FloorNumber, toBuildingObject.FloorNumber);
        return Enumerable.Range(start, end - start + 1);
    }

}