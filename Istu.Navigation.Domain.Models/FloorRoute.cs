using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Models;

public class FloorRoute
{
    public required List<BuildingObject> Objects { get; set; }
    
    public required BuildingObject StartObject { get; set; }
    public required BuildingObject FinishObject { get; set; }
}