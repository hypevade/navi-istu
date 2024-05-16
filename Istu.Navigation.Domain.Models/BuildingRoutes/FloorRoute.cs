namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class FloorRoute(Floor floor, List<BuildingObject> route)
{
    public List<BuildingObject> Route { get; set; } = route;
    public Floor Floor { get; set; } = floor;
}