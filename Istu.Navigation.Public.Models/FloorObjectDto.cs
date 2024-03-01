namespace Istu.Navigation.Public.Models;

public class FloorObjectDto
{
    public required Guid Id { get; set; }
    public required Guid BuildingId { get; set; }
    public required int FloorNumber { get; set; }
    
    public required InnerObjectType ObjectType { get; set; }
    public required string Title { get; set; }
    public List<EdgeDto> Edges { get; set; } = new();
    public double X { get; set; }
    public double Y { get; set; }
}