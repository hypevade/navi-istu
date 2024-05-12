namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class BuildingObject
{
    public BuildingObject(Guid id, Guid buildingId, int floor, BuildingObjectType type, double x, double y, string? title, string? description = null)
    {
        Id = id;
        BuildingId = buildingId;
        Title = title;
        Description = description;
        Floor = floor;
        Type = type;
        X = x;
        Y = y;
    }

    public Guid Id { get; init; }

    public Guid BuildingId { get; init; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int Floor { get; set; }

    public BuildingObjectType Type { get; set; }

    public double X { get; set; }


    public double Y { get; set; }

    public static bool CoordinateIsValid(double coordinate) => coordinate is >= 0 and <= 1;
}