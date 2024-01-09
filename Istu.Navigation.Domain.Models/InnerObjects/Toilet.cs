namespace Istu.Navigation.Domain.Models;

public class Toilet(Guid id, string title, Guid buildingId, int floor,  double x, double y)
    : InnerObject(id, InnerObjectType.Toilet, title, buildingId, floor, x, y);