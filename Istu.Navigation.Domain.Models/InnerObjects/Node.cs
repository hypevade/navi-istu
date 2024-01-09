namespace Istu.Navigation.Domain.Models;

public class Node(Guid id, string title, Guid buildingId, int floor,  double x, double y)
    : InnerObject(id, InnerObjectType.Node, title, buildingId, floor, x, y);