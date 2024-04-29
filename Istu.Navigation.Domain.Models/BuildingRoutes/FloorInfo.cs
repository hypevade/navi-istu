namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class FloorInfo
{
    public FloorInfo(Guid floorId,int floorNumber, string imageLink)
    {
        FloorId = floorId;
        ImageLink = imageLink;
        FloorNumber = floorNumber;
    }
    
    public Guid FloorId { get; set; }
    public int FloorNumber { get; set; }
    public string ImageLink { get; set; }
}