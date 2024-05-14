namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class BuildingObject(
    Guid id,
    Guid buildingId,
    int floor,
    BuildingObjectType type,
    double x,
    double y,
    string? title,
    string? description = null,
    string? keywords = null)
{
    public Guid Id { get; init; } = id;

    public Guid BuildingId { get; init; } = buildingId;

    public string? Title { get; set; } = title;

    public string? Description { get; set; } = description;
    public string? Keywords { get; set; } = keywords;

    public int Floor { get; set; } = floor;

    public BuildingObjectType Type { get; set; } = type;

    public double X { get; set; } = x;


    public double Y { get; set; } = y;

    public static bool CoordinateIsValid(double coordinate) => coordinate is >= 0 and <= 1;
}