
namespace Istu.Navigation.Domain.Models;
public class Cafe(Guid id, string title, double x, double y)
    : InnerObject(id, InnerObjectType.Cafe, title, x, y)
{
    
}