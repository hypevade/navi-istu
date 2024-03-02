namespace Istu.Navigation.Public.Models;

public class BuildingRouteResponse
{
    public required Guid RouteId { get; set; }
    
    public required string BuildingName { get; set; }
    public required Guid BuildingId { get; set; }
    public required List<FloorDto> Floors { get; set; }
    
    //Запасаные, возможно не будут использоваться
    public FloorObjectDto? StartObjectDto { get; set; }
    public FloorObjectDto? FinishObjectDto { get; set; }
    
    public DateTime? CreationDate { get; set; }
    public Guid? CreatedByUser { get; set; }
}