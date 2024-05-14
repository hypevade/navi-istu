using Istu.Navigation.Public.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models.Buildings;

public class FloorDto
{
    public required Guid BuildingId { get; set; }
    public required Guid FloorId { get; set; }
    public required int FloorNumber { get; set; }
    public required List<BuildingObjectDto> Objects { get; set; }
    public required List<EdgeDto> Edges { get; set; }
}