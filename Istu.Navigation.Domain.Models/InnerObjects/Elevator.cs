namespace Istu.Navigation.Domain.Models;

public class Elevator(Guid id, string title, double x, double y)
    : InnerObject(id, InnerObjectType.Elevator, title, x, y)
{
    
}