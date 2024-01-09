
namespace Istu.Navigation.Domain.Models;

public class Cabinet(Guid id, string title, double x, double y)
    : InnerObject(id, InnerObjectType.Cabinet, title, x, y)
{
    
}