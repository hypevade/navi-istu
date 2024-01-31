namespace Istu.Navigation.Domain.Models;

public class Route(InnerObject from, InnerObject to, List<InnerObject>? objects = null)
{
    public required InnerObject From { get; init; } = from;
    public required InnerObject To { get; init; } = to;

    public List<InnerObject>? Objects { get; set; } = objects;
}