using Istu.Navigation.Domain.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models;

public class CreateBuildingObjectRequest
{
    public required Guid BuildingId { get; set; }
    public required int Floor { get; set; }

    public required BuildingObjectType Type { get; set; }
    public required string Title { get; set; }

    public required double X { get; set; }
    public required double Y { get; set; }

    public BuildingObject ToBuildingObject(Guid objectId)
        =>
            new BuildingObject(objectId, BuildingId, Title, Floor, Type,
                X, Y);
}