namespace Istu.Navigation.Domain.Models.InnerObjects;

public class Elevator(Guid id, string title, Guid buildingId, int floor, double x, double y)
    : InnerObject(id, InnerObjectType.Elevator, title, buildingId, floor, x, y)
{
    
}