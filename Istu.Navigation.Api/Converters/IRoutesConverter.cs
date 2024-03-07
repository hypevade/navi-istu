using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Public.Models;

namespace Istu.Navigation.Api.Converters;

public interface IRoutesConverter
{
    public BuildingRouteResponse ConvertToPublicRoute(BuildingRoute internalBuildingRoute); 
}