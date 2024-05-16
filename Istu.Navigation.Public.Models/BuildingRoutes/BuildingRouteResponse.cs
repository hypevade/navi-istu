namespace Istu.Navigation.Public.Models.BuildingRoutes;

public class BuildingRouteResponse
{ 
    public required Guid BuildingId { get; set; }
    public required List<FloorRouteDto> Floors { get; set; }
}