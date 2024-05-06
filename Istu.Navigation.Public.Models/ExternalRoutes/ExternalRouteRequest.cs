using Istu.Navigation.Domain.Models.ExternalRoutes;

namespace Istu.Navigation.Public.Models.ExternalRoutes;

public class ExternalRouteRequest
{
    public required ExternalPointDto StartPointDto { get; set; }
    public required Guid BuildingId { get; set; }
    public required ExternalRouteType Type { get; set; }
}