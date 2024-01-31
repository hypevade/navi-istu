namespace Istu.Navigation.Domain.Models;
public abstract class InnerObject(Guid id, InnerObjectType type, string title, Guid buildingId, int floor, double x, double y)
{
    public Guid Id { get; set; } = id;
    public string Title { get; set; } = title;
    public int Floor { get; } = floor;
    public Guid BuildingId { get; } = buildingId;
    public InnerObjectType Type { get; set; } = type;
    
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public List<InnerObject>? ConnectedObjects { get; set; } = null;
}