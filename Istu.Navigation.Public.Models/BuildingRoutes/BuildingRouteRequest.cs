namespace Istu.Navigation.Public.Models.BuildingRoutes;

public class BuildingRouteRequest
{
    public required Guid BuildingId { get; set; }
    public required Guid ToId { get; set; }
    public Guid? FromId { get; set; }
}