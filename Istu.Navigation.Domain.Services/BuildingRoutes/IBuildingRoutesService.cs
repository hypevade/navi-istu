﻿using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services.BuildingRoutes;

public interface IBuildingRoutesService
{
    Task<OperationResult<BuildingRoute>> CreateRouteAsync(Guid toId, Guid fromId = default, bool enableElevator = false);
}
public class BuildingRoutesService : IBuildingRoutesService
{
    private readonly IBuildingObjectsService objectService;
    private readonly IBuildingsService service;
    private readonly IRouteSearcher searcher;
    private readonly IFloorsBuilder floorsBuilder;

    public BuildingRoutesService(IBuildingObjectsService objectService,
        IBuildingsService service,
        IRouteSearcher searcher,
        IFloorsBuilder floorsBuilder)
    {
        this.objectService = objectService;
        this.service = service;
        this.searcher = searcher;
        this.floorsBuilder = floorsBuilder;
    }

    public async Task<OperationResult<BuildingRoute>> CreateRouteAsync(Guid toId, Guid fromId = default, bool enableElevator = true)
    {
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

        var edges = getFloors.Data.SelectMany(x => x.Edges).ToList();
        var objects = edges
            .SelectMany(edge => new[] { edge.From, edge.To })
            .Distinct().ToList();
        
        if (!enableElevator)
            objects = objects.Where(x => x.Type != BuildingObjectType.Elevator).ToList();
        
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
            if (!routes.TryGetValue(floor.FloorNumber, out var route))
                route = new List<BuildingObject>();
            
            floorRoutes.Add(new FloorRoute(floor, route));
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