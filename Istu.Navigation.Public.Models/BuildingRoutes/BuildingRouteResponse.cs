namespace Istu.Navigation.Public.Models.BuildingRoutes;

public class BuildingRouteResponse
{
    public required Guid RouteId { get; set; }
    
    public required string BuildingName { get; set; }
    public required Guid BuildingId { get; set; }
    
    public required List<FloorDto> Floors { get; set; }
    
    //Запасаные, возможно не будут использоваться
    public BuildingObjectDto? StartObjectDto { get; set; }
    public BuildingObjectDto? FinishObjectDto { get; set; }
    
    //public Guid? CreatedByUser { get; set; }
}