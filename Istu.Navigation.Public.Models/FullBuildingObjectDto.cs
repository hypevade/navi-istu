namespace Istu.Navigation.Public.Models;

public class FullBuildingObjectDto
{
    public required Guid Id { get; set; }
    public required Guid BuildingId { get; set; }
    public required int FloorNumber { get; set; }
    
    public required PublicObjectType Type { get; set; }
    public required string Title { get; set; }
    
    public string? Description { get; set; }

    public double X { get; set; }
    public double Y { get; set; }
    
    //Cвойства, которые будут нужны в будущем 
    //public List<string>? Images { get; set; }
    //public List<CommentDto>? Comments { get; set; }
}