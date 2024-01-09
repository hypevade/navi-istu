namespace Istu.Navigation.Domain.Models;

public class Ladder(Guid id, string title, double x, double y)
    : InnerObject(id, InnerObjectType.Ladder, title, x, y)
{
    
}