namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class FloorInfo(Guid floorId, int floorNumber)
{
    public Guid FloorId { get; set; } = floorId;
    public int FloorNumber { get; set; } = floorNumber;
}