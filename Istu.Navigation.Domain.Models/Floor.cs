namespace Istu.Navigation.Domain.Models;

public class Floor(List<InnerObject> objects)
{
    public List<InnerObject> Objects { get; set; } = objects;
}