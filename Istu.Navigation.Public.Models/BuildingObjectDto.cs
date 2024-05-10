using Istu.Navigation.Domain.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models;

public class BuildingObjectDto
{
    public required Guid Id { get; set; }
    public required Guid BuildingId { get; set; }
    public required int Floor { get; set; }
    
    public required BuildingObjectType Type { get; set; }
    public required string Title { get; set; }
    
    public string? Description { get; set; }

    public double X { get; set; }
    public double Y { get; set; }
    
    //Cвойства, которые будут нужны в будущем 
    //public List<string>? Images { get; set; }
    //public List<CommentDto>? Comments { get; set; }
}