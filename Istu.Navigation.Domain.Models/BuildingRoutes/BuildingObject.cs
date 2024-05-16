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

    public double X { get; init; } = x;
    
    public double Y { get; init; } = y;

    public override bool Equals(object? obj)
    {
        return obj is BuildingObject other && Equals(other);
    }

    private bool Equals(BuildingObject other)
    {
        return Id.Equals(other.Id) && BuildingId.Equals(other.BuildingId) && Title == other.Title &&
               Description == other.Description && Keywords == other.Keywords && Floor == other.Floor &&
               Type == other.Type && X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(BuildingId);
        hashCode.Add(X);
        hashCode.Add(Y);
        return hashCode.ToHashCode();
    }

    public static bool CoordinateIsValid(double coordinate) => coordinate is >= 0 and <= 1;
}