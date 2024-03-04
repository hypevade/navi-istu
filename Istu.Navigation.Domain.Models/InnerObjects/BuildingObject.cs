namespace Istu.Navigation.Domain.Models.InnerObjects;

public class BuildingObject
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public int Floor { get; init; }
    public required Guid BuildingId { get; init; }
    public BuildingObjectType Type { get; init; }
    
    public required double X { get; set; }
    public required double Y { get; set; }
}

public class FloorObject
{
    public required BuildingObject BuildingObject { get; set; }
    public List<Edge> Edges { get; } = new();
    
    public void AddEdge(BuildingObject targetObject, double weight)
    {
        //Todo: может быть тут понадобиться проверять на существование ребра
        var edge = new Edge(BuildingObject, targetObject);
        Edges.Add(edge);
    }
    
    public void DeleteEdge(Guid targetObjectId)
    {
        var targetObject = Edges.FirstOrDefault(edge => edge.To.Id == targetObjectId);
        if (targetObject is not null)
            Edges.Remove(targetObject);
    }
}