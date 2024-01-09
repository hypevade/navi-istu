namespace Istu.Navigation.Domain.Models;

public class Entrance(Guid id, string title, Guid buildingId, int floor,  double x, double y)
    : InnerObject(id, InnerObjectType.Entrance, title, buildingId, floor, x, y)
{
    
}