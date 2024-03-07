using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services;

public class BuildingRoutesService(IBuildingObjectsRepository buildingObjectRepository, IBuildingsRepository buildingsRepository, IRouteSearcher routeSearcher, IImageService imageService, IRouteRepository routeRepository, IEdgesRepository edgesRepository)
{
    private IBuildingObjectsRepository buildingObjectRepository = buildingObjectRepository;
    private IBuildingsRepository buildingsRepository = buildingsRepository;
    private IRouteRepository routeRepository = routeRepository;
    private IEdgesRepository edgesRepository = edgesRepository;
    
    private IRouteSearcher routeSearcher = routeSearcher;
    private IImageService imageService = imageService;
    public async Task<OperationResult<BuildingRoute>> CreateRoute(Guid buildingId, Guid toId, Guid fromId = default)
    {
        //TODO: Добавить  поддержку, когда fromID = default
        
        var getToObject = await buildingObjectRepository.GetById(toId).ConfigureAwait(false);
        if (getToObject.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getToObject.ApiError);
        
        var getFromObject = await buildingObjectRepository.GetById(fromId).ConfigureAwait(false);
        if (getFromObject.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFromObject.ApiError);

        var toObject = getToObject.Data;
        var fromObject = getFromObject.Data;

        var getFloors = await GetFloors(fromObject, toObject, buildingId).ConfigureAwait(false);
        if(getFloors.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getFloors.ApiError);
        
        //TODO: неправильно работает, так как from и toObject на каждом этаже разные
        var getFloorRoutes = getFloors.Data.Select(floor => routeSearcher.CreateRoute(fromObject, toObject, floor)).ToList();
        
        if(getFloorRoutes.Any(x=>x.IsFailure))
            return OperationResult<BuildingRoute>.Failure(getFloorRoutes.First(x=>x.IsFailure).ApiError);
        
        var getBuilding = await buildingsRepository.GetById(buildingId).ConfigureAwait(false);
        if(getBuilding.IsFailure)
            return OperationResult<BuildingRoute>.Failure(getBuilding.ApiError);
        
        var floors = getFloors.Data;
        var floorRoutes = getFloorRoutes.Select(x => x.Data).ToList();
        var building = getBuilding.Data;

        var resultRoute = new BuildingRoute(building, floorRoutes, startObject: fromObject, finishObject: toObject);
        
        return OperationResult<BuildingRoute>.Success(resultRoute);
        
        /*var createRoute = await routeRepository.CreateRoute(resultRoute).ConfigureAwait(false);

        return createRoute.IsFailure 
            ? OperationResult<BuildingRoute>.Failure(createRoute.ApiError) 
            : OperationResult<BuildingRoute>.Success(resultRoute);*/
    }
    
    public async Task<OperationResult<BuildingRoute>> GetRouteById(Guid routeId)
    {
        var route = await routeRepository.GetById(routeId).ConfigureAwait(false);
        return route;
    }
    

    private async Task<OperationResult<List<Floor>>> GetFloors(BuildingObject fromBuildingObject, BuildingObject toBuildingObject, Guid buildingId)
    {
        var numbersOfFloors = GetFloorsNumbers(fromBuildingObject, toBuildingObject);
        var floors = new List<Floor>();
        foreach (var floorNumber in numbersOfFloors)
        {
            var getObjects = await buildingObjectRepository.GetAllByFloor(buildingId, floorNumber)
                .ConfigureAwait(false);
            if (getObjects.IsFailure)
                return OperationResult<List<Floor>>.Failure(getObjects.ApiError);

            var getFloorImageLink = await imageService.GetImageLink(buildingId, floorNumber)
                .ConfigureAwait(false);
            
            if (!getFloorImageLink.IsSuccess)
                return OperationResult<List<Floor>>.Failure(getFloorImageLink.ApiError);
            
            var getEdges = await edgesRepository.GetAllByFloor(buildingId, floorNumber).ConfigureAwait(false);
            if (getEdges.IsFailure)
                return OperationResult<List<Floor>>.Failure(getEdges.ApiError);
            var edges = getEdges.Data.ToList();
            var objects = getObjects.Data.ToList();
            
            var floor = new Floor(Guid.NewGuid(), buildingId, floorNumber, objects,edges, getFloorImageLink.Data);
            floors.Add(floor);
        }
        return OperationResult<List<Floor>>.Success(floors);
    }

    private IEnumerable<int> GetFloorsNumbers(BuildingObject fromBuildingObject, BuildingObject toBuildingObject)
    {
        var start = Math.Min(fromBuildingObject.Floor, toBuildingObject.Floor);
        var end = Math.Max(fromBuildingObject.Floor, toBuildingObject.Floor);
        return Enumerable.Range(start, end - start + 1);
    }

}