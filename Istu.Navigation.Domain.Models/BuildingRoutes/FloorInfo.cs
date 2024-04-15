namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class FloorInfo
{
    public FloorInfo(int floorNumber, string imageLink)
    {
        ImageLink = imageLink;
        FloorNumber = floorNumber;
    }
    
    public int FloorNumber { get; set; }
    public string ImageLink { get; set; }
}