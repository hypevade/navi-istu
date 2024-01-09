namespace Istu.Navigation.Domain.Models;

public class Toilet(Guid id, string title, double x, double y)
    : InnerObject(id, InnerObjectType.Toilet, title, x, y)
{
    
}