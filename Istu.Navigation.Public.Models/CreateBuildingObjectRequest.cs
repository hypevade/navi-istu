using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models;

public class CreateBuildingObjectRequest
{
    public required Guid BuildingId { get; set; }
    public required int FloorNumber { get; set; }

    public required BuildingObjectType Type { get; set; }
    public required string Title { get; set; }

    public double X { get; set; }
    public double Y { get; set; }

    public BuildingObject ToBuildingObject(Guid objectId)
        =>
            new BuildingObject(objectId, BuildingId, Title, FloorNumber, Type,
                X, Y);

}