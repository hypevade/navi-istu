namespace Istu.Navigation.Domain.Models.InnerObjects;

public abstract class InnerObject(Guid id, InnerObjectType type, string title, Guid buildingId, int floor, double x, double y)
{
    public Guid Id { get; set; } = id;
    public string Title { get; set; } = title;
    public int Floor { get; } = floor;
    public Guid BuildingId { get; } = buildingId;
    public InnerObjectType Type { get; set; } = type;
    
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public List<Edge> Edges { get; set; } = new();
    
    public void AddEdge(InnerObject targetObject, double weight)
    {
        //Todo: может быть тут понадобиться проверять на существование ребра
        var edge = new Edge(this, targetObject, weight);
        Edges.Add(edge);
    }
    public void DeleteEdge(Guid targetObjectId)
    {
        var targetObject = Edges.FirstOrDefault(edge => edge.To.Id == targetObjectId);
        if (targetObject is not null)
            Edges.Remove(targetObject);
    }
}