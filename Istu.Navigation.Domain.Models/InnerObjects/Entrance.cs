namespace Istu.Navigation.Domain.Models;

public class Entrance(Guid id, string title, double x, double y)
    : InnerObject(id, InnerObjectType.Entrance, title, x, y)
{
    
}