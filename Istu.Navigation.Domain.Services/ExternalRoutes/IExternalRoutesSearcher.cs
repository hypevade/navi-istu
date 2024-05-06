using Istu.Navigation.Domain.Models.ExternalRoutes;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.ExternalRoutesApiErrors;
using Itinero;
using Itinero.LocalGeo;
using Itinero.Profiles;
using Microsoft.Extensions.Options;
using Vehicle = Itinero.Osm.Vehicles.Vehicle;

namespace Istu.Navigation.Domain.Services.ExternalRoutes;

public interface IExternalRoutesSearcher
{
    public OperationResult<ExternalRoute> FindRoute(ExternalPoint startPoint, ExternalPoint endPoint, ExternalRouteType type); 
}

public class ExternalRoutesSearcher(Router router, IOptions<MapOptions> options) : IExternalRoutesSearcher
{
    private readonly MapOptions mapOptions = options.Value;
    public OperationResult<ExternalRoute> FindRoute(ExternalPoint startPoint, ExternalPoint endPoint, ExternalRouteType type)
    {
        if (!CheckPoint(startPoint))
            return OperationResult<ExternalRoute>.Failure(ExternalRoutesApiError.StartPointOutsideAreaError(startPoint.Latitude, startPoint.Longitude));
        
        if (!CheckPoint(endPoint))
            return OperationResult<ExternalRoute>.Failure(ExternalRoutesApiError.EndPointOutsideAreaError(endPoint.Latitude, endPoint.Longitude));
        
        var profile = Vehicle.Pedestrian.Shortest();
        if (type == ExternalRouteType.Bicycle)
            profile = Vehicle.Bicycle.Shortest();
        
        var start = ToRouterPoint(startPoint, profile);
        var end = ToRouterPoint(endPoint, profile);
        var route = router.Calculate(profile, start, end);

        var externalRoute = ToExternalRoute(route, type);
        return OperationResult<ExternalRoute>.Success(externalRoute);
    }
    private bool CheckPoint(ExternalPoint point)
    {
        if (point.Latitude <= mapOptions.MinLongitude || point.Latitude >= mapOptions.MaxLatitude)
            return false;
        if (point.Longitude <= mapOptions.MinLongitude || point.Longitude >= mapOptions.MaxLongitude)
            return false;
        return true;
    }

    private RouterPoint? ToRouterPoint(ExternalPoint point, IProfileInstance profile)
    {
        return router.Resolve(profile, (float)point.Latitude, (float)point.Longitude);
    }

    private ExternalRoute ToExternalRoute(Route route, ExternalRouteType routeType)
    {
        var points = route.Shape.Select(ToExternalPoint).ToList();
        var time = route.TotalTime;
        return new ExternalRoute(points, TimeSpan.FromSeconds(time), routeType);
    }

    private ExternalPoint ToExternalPoint(Coordinate point)
    {
        return new ExternalPoint(point.Latitude, point.Longitude);
    }
}