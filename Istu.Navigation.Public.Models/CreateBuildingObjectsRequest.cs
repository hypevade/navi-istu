namespace Istu.Navigation.Public.Models;

public class CreateBuildingObjectsRequest
{
    public required List<FullBuildingObjectDto> BuildingObjects { get; set; }
}