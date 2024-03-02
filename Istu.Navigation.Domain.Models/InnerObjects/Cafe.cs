
namespace Istu.Navigation.Domain.Models.InnerObjects;
public class Cafe(Guid id, string title, Guid buildingId, int floor, double x, double y)
    : InnerObject(id, InnerObjectType.Cafe, title, buildingId, floor, x, y);