namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class FloorRoute
{
    public FloorRoute(List<BuildingObject> route, Floor floor, BuildingObject startObject, BuildingObject finishObject)
    {
        Route = route;
        Floor = floor;
        StartObject = startObject;
        FinishObject = finishObject;
    }

    public List<BuildingObject> Route { get; set; }
    public Floor Floor { get; set; }
    public BuildingObject StartObject { get; set; }
    public BuildingObject FinishObject { get; set; }
}