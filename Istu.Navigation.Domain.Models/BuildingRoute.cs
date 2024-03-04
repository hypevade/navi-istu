using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Models;

public class BuildingRoute
{
    public required Guid RouteId { get; set; }
    
    public required string BuildingTitle { get; set; }
    public required Guid BuildingId { get; set; }
    public required List<Floor> Floors { get; set; }
    public required List<FloorRoute> FloorRoutes { get; set; }
    
    //Запасаные, возможно не будут использоваться
    public BuildingObject? StartObject { get; set; }
    public BuildingObject? FinishObject { get; set; }
    
    public DateTime? CreationDate { get; set; }
    public Guid? CreatedByUser { get; set; }
}