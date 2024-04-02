using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class Floor
{
    public Floor( Guid buildingId, int floorNumber, List<BuildingObject> objects, List<Edge> edges, ImageLink imageLink)
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

    public ImageLink ImageLink { get; set; }

    public static FloorEntity ToDto(Floor floor)
    {
        return new FloorEntity
        {
            BuildingId = floor.BuildingId,
            FloorNumber = floor.FloorNumber,
            ImageId = floor.ImageLink.Id,
            IsDeleted = false
        };
    }
}