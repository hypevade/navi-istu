namespace Istu.Navigation.Domain.Models.InnerObjects;

public class Auditorium(Guid id, string title, Guid buildingId, int floor, double x, double y)
    : InnerObject(id, InnerObjectType.Auditorium, title, buildingId, floor, x, y)
{
}