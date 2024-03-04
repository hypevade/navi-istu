using Istu.Navigation.Domain.Models;

namespace Istu.Navigation.Domain.Repositories;

public interface IRouteRepository
{
    public Task<List<BuildingRoute>> GetAll();
    public Task<BuildingRoute> GetById(Guid routeId);
    
    //TODO: будет возвращать что-то типа APIOperationResult
    public Task CreateRoute(BuildingRoute route);
}