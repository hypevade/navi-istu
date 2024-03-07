using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class Edge
{
    public Edge(Guid id, BuildingObject from, BuildingObject to)
    {
        Id = id;
        From = from;
        To = to;
    }

    public Guid Id { get; init; }
    public BuildingObject From { get; init; }
    public BuildingObject To { get; init; }
    public static EdgeDto ToDto(Edge edge)
    {
        return new EdgeDto
        {
            Id = edge.Id,
            FromObjectId = edge.From.Id,
            ToObjectId = edge.To.Id
        };
    }
}