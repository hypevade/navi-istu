namespace Istu.Navigation.Domain.Models;

public class Ladder(Guid id, string title, Guid buildingId, int floor, double x, double y)
    : InnerObject(id, InnerObjectType.Ladder, title, buildingId, floor, x, y);