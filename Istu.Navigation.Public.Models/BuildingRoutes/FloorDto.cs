namespace Istu.Navigation.Public.Models.BuildingRoutes;

public class FloorDto
{
    public required Guid BuildingId { get; set; }
    public required int Number { get; set; }
    public required string ImageLink { get; set; }

    public required List<BuildingObjectDto> Objects { get; set; }
    public required List<EdgeDto> Edges { get; set; }

    public required List<BuildingObjectDto> Route { get; set; }
}