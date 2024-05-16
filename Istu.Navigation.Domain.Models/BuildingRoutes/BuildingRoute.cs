namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class BuildingRoute(Building building, List<FloorRoute> floorRoutes)
{
    public Building Building { get; set; } = building;
    public List<FloorRoute> FloorRoutes { get; set; } = floorRoutes;
}