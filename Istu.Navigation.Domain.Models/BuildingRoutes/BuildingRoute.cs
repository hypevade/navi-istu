namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class BuildingRoute
{
    public BuildingRoute(Building building, List<FloorRoute> floorRoutes, Guid? routeId = null,
        BuildingObject? startObject = null, BuildingObject? finishObject = null)
    {
        RouteId = routeId;
        Building = building;
        FloorRoutes = floorRoutes;
        StartObject = startObject;
        FinishObject = finishObject;
    }

    public Guid? RouteId { get; set; }
    public Building Building { get; set; }
    public List<FloorRoute> FloorRoutes { get; set; }
    
    //Запасаные, возможно не будут использоваться
    public BuildingObject? StartObject { get; set; }
    public BuildingObject? FinishObject { get; set; }
    
    /*
    public DateTime? CreationDate { get; set; }
    public Guid? CreatedByUser { get; set; }*/
}