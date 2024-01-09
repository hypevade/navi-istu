
namespace Istu.Navigation.Domain.Models;

public class Cabinet(Guid id, string title, Guid buildingId, int floor, double x, double y)
    : InnerObject(id, InnerObjectType.Cabinet, title, buildingId, floor, x, y)
{
    
}