using Istu.Navigation.Domain.Models;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IRouteRepository
{
    public Task<OperationResult<List<BuildingRoute>>> GetAll();
    public Task<OperationResult<BuildingRoute>> GetById(Guid routeId);
    public Task<OperationResult> CreateRoute(BuildingRoute route);
}