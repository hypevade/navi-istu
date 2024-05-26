using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.Buildings;

namespace Istu.Navigation.Public.Models;

public class CreateBuildingObjectRequest
{
    public required Guid BuildingId { get; set; }

    public required int Floor { get; set; }

    public required BuildingObjectType Type { get; set; }
    public string? Title { get; set; }

    public string? Keywords { get; set; }
    public string? Description { get; set; }

    public required BuildingPositionDto Position { get; set; }

    public BuildingObject ToBuildingObject(Guid objectId)
        => new(objectId, BuildingId, Floor, Type, Position.X,
            Position.Y, Title, keywords: Keywords, description: Description);
}