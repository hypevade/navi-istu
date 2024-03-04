using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Models;

public class Floor
{
    public required Guid BuildingId { get; set; }
    public required int Number { get; set; } 
    
    public required List<BuildingObject> Objects { get; set; }
    public required List<Edge> Edges { get; set; }
    //TODO: еще 10 раз подумать нужно ли это поле
    public FloorRoute? FloorRoute { get; set; }
    
    public required string ImageLink { get; set; }
}