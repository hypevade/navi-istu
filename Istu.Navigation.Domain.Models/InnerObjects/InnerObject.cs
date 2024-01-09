namespace Istu.Navigation.Domain.Models;
public abstract class InnerObject(Guid id, InnerObjectType type, string title, double x, double y)
{
    public Guid Id { get; set; } = id;
    public string Title { get; set; } = title;
    public InnerObjectType Type { get; set; } = type;
    
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
}