namespace Istu.Navigation.Public.Models.ExternalRoutes;

public class ExternalRouteRequest
{
    public required ExternalRoutePoint StartPoint { get; set; }
    public required Guid BuildingId { get; set; }
}