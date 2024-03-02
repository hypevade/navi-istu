using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Domain.Repositories;

namespace Istu.Navigation.Domain.Services;

public class InnerRoutesService(IInnerObjectsRepository innerObjectRepository, IBuildingsRepository buildingsRepository, IRouteSearcher routeSearcher)
{
    private IInnerObjectsRepository innerObjectRepository = innerObjectRepository;
    private IBuildingsRepository buildingsRepository = buildingsRepository;
    private IRouteSearcher routeSearcher = routeSearcher;

    public async Task<List<(Floor, Route)>> CreateRoute(Guid buildingId, Guid toId, Guid fromId = default)
    {
        //TODO: Добавить  поддержку, когда fromID = default
        var toObject = await innerObjectRepository.GetById(toId).ConfigureAwait(false);
        var fromObject = await innerObjectRepository.GetById(fromId).ConfigureAwait(false);
        //TODO: Сделать нормальное прокидование ошибки 
        if (toObject is null || fromObject is null)
            throw new KeyNotFoundException();

        var floors = await GetFloors(fromObject, toObject).ConfigureAwait(false);
        
        var tasks = floors.Select(floor => routeSearcher.CreateRoute(fromObject, toObject, floor)).ToList();
        await Task.WhenAll(tasks).ConfigureAwait(false);
        var result = tasks.Select(task => task.Result).ToList();
        return result;
    }
    
    public async GetRouteById(Guid routeId)
    {
        //TODO: Сделать нормальное прокидование ошибки 
        var route = await routeSearcher.CreateRoute(routeId).ConfigureAwait(false);
        return route;
    }
    

    private async Task<IEnumerable<Floor>> GetFloors(InnerObject fromInnerObject, InnerObject toInnerObject)
    {
        var numbersOfFloors = GetFloorsNumbers(fromInnerObject, toInnerObject);
        var floors = new List<Floor>();
        foreach (var floorNumber in numbersOfFloors)
        {
            var objects = await innerObjectRepository.GetAllByFloor(fromInnerObject.BuildingId, floorNumber)
                .ConfigureAwait(false);
            var floor = new Floor(objects.ToList());
            floors.Add(floor);
        }
        return floors;
    }

    private IEnumerable<int> GetFloorsNumbers(InnerObject fromInnerObject, InnerObject toInnerObject)
    {
        var start = Math.Min(fromInnerObject.Floor, toInnerObject.Floor);
        var end = Math.Max(fromInnerObject.Floor, toInnerObject.Floor);
        return Enumerable.Range(start, end - start + 1);
    }

}