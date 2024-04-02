namespace Istu.Navigation.Public.Models;

public class BuildingObjectDto
{
    public required Guid Id { get; set; }
    public required Guid BuildingId { get; set; }
    public required int FloorNumber { get; set; }
    
    public required PublicObjectType Type { get; set; }
    public required string Title { get; set; }
    
    public double X { get; set; }
    public double Y { get; set; }
}