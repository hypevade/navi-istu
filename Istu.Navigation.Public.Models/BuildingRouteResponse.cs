namespace Istu.Navigation.Public.Models;

public class BuildingRouteResponse
{
    public required string BuildingName { get; set; }
    public required string BuildingId { get; set; }
    public required List<FloorDto> Floors { get; set; }
    
    //Запасаные, возможно не будут использоваться
    public FloorObjectDto? StartObjectDto { get; set; }
    public FloorObjectDto? FinishObjectDto { get; set; }
    
}