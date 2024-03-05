using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Domain.Repositories;

namespace Istu.Navigation.Domain.Services;

public class BuildingRoutesService(IBuildingObjectsRepository buildingObjectRepository, IBuildingsRepository buildingsRepository, IRouteSearcher routeSearcher, IImageService imageService, IRouteRepository routeRepository, IEdgesRepository edgesRepository)
{
    private IBuildingObjectsRepository buildingObjectRepository = buildingObjectRepository;
    private IBuildingsRepository buildingsRepository = buildingsRepository;
    private IRouteRepository routeRepository = routeRepository;
    private IEdgesRepository edgesRepository = edgesRepository;
    
    private IRouteSearcher routeSearcher = routeSearcher;
    private IImageService imageService = imageService;

    public async Task<BuildingRoute> CreateRoute(Guid userId, Guid buildingId, Guid toId, Guid fromId = default)
    {
        //TODO: Добавить  поддержку, когда fromID = default
        
        var toObject = await buildingObjectRepository.GetById(toId).ConfigureAwait(false);
        var fromObject = await buildingObjectRepository.GetById(fromId).ConfigureAwait(false);
        
        //TODO: Сделать нормальное прокидование ошибки 
        if (toObject is null || fromObject is null)
            throw new KeyNotFoundException();

        var floors = await GetFloors(fromObject, toObject, buildingId).ConfigureAwait(false);
        var floorRoutes = floors.Select(floor => routeSearcher.CreateRoute(fromObject, toObject, floor)).ToList();

        var building = await buildingsRepository.GetById(buildingId).ConfigureAwait(false);
        
        var resultRoute = new BuildingRoute()
        {
            BuildingId = buildingId,
            BuildingTitle = building.Title,
            Floors = floors,
            FloorRoutes = floorRoutes.Select(x=>x.Data).ToList(),
            CreationDate = DateTime.UtcNow,
            CreatedByUser = userId,
            RouteId = Guid.NewGuid(),
            StartObject = fromObject,
            FinishObject = toObject
        };
        
        await routeRepository.CreateRoute(resultRoute).ConfigureAwait(false);

        return resultRoute;
    }
    
    public async Task<BuildingRoute> GetRouteById(Guid routeId)
    {
        var route = await routeRepository.GetById(routeId).ConfigureAwait(false);
        return route;
    }
    

    private async Task<List<Floor>> GetFloors(BuildingObject fromBuildingObject, BuildingObject toBuildingObject, Guid buildingId)
    {
        var numbersOfFloors = GetFloorsNumbers(fromBuildingObject, toBuildingObject);
        var floors = new List<Floor>();
        foreach (var floorNumber in numbersOfFloors)
        {
            var objects = await buildingObjectRepository.GetAllByFloor(buildingId, floorNumber)
                .ConfigureAwait(false);

            var floorImageLink = await imageService.GetFloorImageLink(buildingId, floorNumber)
                .ConfigureAwait(false);
            var edges = await edgesRepository.GetAllByFloor(buildingId, floorNumber).ConfigureAwait(false);
            var floor = new Floor()
            {
                BuildingId = buildingId,
                Number = floorNumber,
                Objects = objects.ToList(),
                ImageLink = floorImageLink, 
                Edges = edges.ToList()
            };
            floors.Add(floor);
        }
        return floors;
    }

    private IEnumerable<int> GetFloorsNumbers(BuildingObject fromBuildingObject, BuildingObject toBuildingObject)
    {
        var start = Math.Min(fromBuildingObject.Floor, toBuildingObject.Floor);
        var end = Math.Max(fromBuildingObject.Floor, toBuildingObject.Floor);
        return Enumerable.Range(start, end - start + 1);
    }

}