namespace Istu.Navigation.Public.Models;

public class FloorDto
{
    public required Guid BuildingId { get; set; }
    public required int Number { get; set; } 
    
    public required List<FloorObjectDto> Objects { get; set; }
    public FloorRouteDto? FloorRoute { get; set; }
    
    public required string ImageLink { get; set; }
}