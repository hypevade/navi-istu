using Istu.Navigation.Public.Models.Buildings;

namespace Istu.Navigation.Public.Models.BuildingRoutes;

public class FloorRouteDto
{
    public required FloorDto FloorDto { get; set; }
    public required List<BuildingObjectDto> Route { get; set; }
}