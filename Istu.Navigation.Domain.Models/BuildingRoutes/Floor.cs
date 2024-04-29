using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class Floor
{
    public Floor( Guid buildingId, int floorNumber, List<BuildingObject> objects, List<Edge> edges, string imageLink)
    {
        BuildingId = buildingId;
        FloorNumber = floorNumber;
        Objects = objects;
        Edges = edges;
        ImageLink = imageLink;
    }
    
    public Guid BuildingId { get; set; }
    public int FloorNumber { get; set; }

    public List<BuildingObject> Objects { get; set; }
    public List<Edge> Edges { get; set; }

    public string ImageLink { get; set; }
}