namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class Edge
{
    public Edge(Guid id, BuildingObject from, BuildingObject to, int floorNumber)
    {
        Id = id;
        From = from;
        To = to;
        FloorNumber = floorNumber;
    }

    public Guid Id { get; init; }
    public Guid BuildingId { get; init; }
    public int FloorNumber { get; init; }
    public BuildingObject From { get; init; }
    public BuildingObject To { get; init; }
}