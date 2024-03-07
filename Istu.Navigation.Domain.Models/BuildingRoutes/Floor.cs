using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class Floor
{
    public Floor(Guid id, Guid buildingId, int number, List<BuildingObject> objects, List<Edge> edges, ImageLink imageLink)
    {
        Id = id;
        BuildingId = buildingId;
        Number = number;
        Objects = objects;
        Edges = edges;
        ImageLink = imageLink;
    }

    public Guid Id { get; set; }
    public Guid BuildingId { get; set; }
    public int Number { get; set; }

    public List<BuildingObject> Objects { get; set; }
    public List<Edge> Edges { get; set; }

    public ImageLink ImageLink { get; set; }

    public static FloorDto ToDto(Floor floor)
    {
        return new FloorDto
        {
            BuildingId = floor.BuildingId,
            Number = floor.Number,
            ImageId = floor.ImageLink.Id,
            Id = floor.Id
        };
    }
}