namespace Istu.Navigation.Public.Models.Buildings;

public class ExternalPositionDto
{
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public static ExternalPositionDto From(double latitude, double longitude) => new() { Latitude = latitude, Longitude = longitude };
}

public class BuildingPositionDto
{
    public required double X { get; set; }
    public required double Y { get; set; }
    public static BuildingPositionDto From(double x, double y) => new() { X = x, Y = y };
}