namespace Istu.Navigation.Public.Models.Buildings;

public class CreateFloorRequest
{
    public required string ImageLink { get; set; }
    public int? FloorNumber { get; set; }
}