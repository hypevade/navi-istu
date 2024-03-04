using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Models;

public class Route(BuildingObject from, BuildingObject to, List<BuildingObject>? objects = null)
{
    public required BuildingObject From { get; init; } = from;
    public required BuildingObject To { get; init; } = to;

    public List<BuildingObject>? Objects { get; set; } = objects;
}