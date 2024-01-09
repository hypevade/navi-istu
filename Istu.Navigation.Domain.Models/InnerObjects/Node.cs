namespace Istu.Navigation.Domain.Models;

public class Node(Guid id, string title, double x, double y)
    : InnerObject(id, InnerObjectType.Node, title, x, y)
{
    
}