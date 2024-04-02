using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services;

public interface IBuildingRoutesService
{
    Task<OperationResult<BuildingRoute>> CreateRoute(Guid buildingId, Guid toId, Guid fromId = default);
    //Task<OperationResult<BuildingRoute>> GetRouteById(Guid routeId);
}