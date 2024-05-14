namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class Floor(Guid buildingId, Guid floorId, int floorNumber, List<BuildingObject> objects, List<Edge> edges)
{
    public Guid BuildingId { get; set; } = buildingId;
    public Guid FloorId { get; set; } = floorId;
    public int FloorNumber { get; set; } = floorNumber;

    public List<BuildingObject> Objects { get; set; } = objects;
    public List<Edge> Edges { get; set; } = edges;
}