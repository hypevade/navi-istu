namespace Istu.Navigation.Domain.Models;

public class Auditorium(Guid id, string title, double x, double y)
    : InnerObject(id, InnerObjectType.Auditorium, title, x, y)
{
}