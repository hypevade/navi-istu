using Istu.Navigation.Domain.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models;

public class CreateBuildingObjectRequest
{
    public required Guid BuildingId { get; set; }
    
    public required int Floor { get; set; }

    public required BuildingObjectType Type { get; set; }
    public string? Title { get; set; }
    
    public string? Keywords { get; set; }

    public required double X { get; set; }
    public required double Y { get; set; }

    public BuildingObject ToBuildingObject(Guid objectId)
        =>
            new(objectId, BuildingId, Floor, Type, X,
                Y, Title);
}