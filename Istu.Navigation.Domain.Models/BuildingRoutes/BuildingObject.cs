namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class BuildingObject
{
    public BuildingObject(Guid id, Guid buildingId, string title, int floorNumber, BuildingObjectType type, double x, double y, string? description = null)
    {
        Id = id;
        BuildingId = buildingId;
        Title = title;
        Description = description;
        FloorNumber = floorNumber;
        Type = type;
        X = x;
        Y = y;
    }

    public Guid Id { get; init; }

    public Guid BuildingId { get; init; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public int FloorNumber
    {
        get => floorNumber;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(FloorNumber));
            floorNumber = value;
        }
    }

    private readonly int floorNumber;

    public BuildingObjectType Type { get; set; }

    public double X
    {
        get => x;
        set
        {
            if (!CoordinatesIsValid(value))
                throw new ArgumentOutOfRangeException(nameof(X), "Координаты должны быть в диапазоне от 0 до 1");
            x = value;
        }
    }

    private double x;


    public double Y
    {
        get => y;
        set
        {
            if (!CoordinatesIsValid(value))
                throw new ArgumentOutOfRangeException(nameof(Y), "Координаты должны быть в диапазоне от 0 до 1");
            y = value;
        }
    }

    private double y;

    public static bool CoordinatesIsValid(double coordinate) => coordinate is >= 0 and <= 1;
}