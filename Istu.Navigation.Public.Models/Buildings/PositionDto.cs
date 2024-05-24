namespace Istu.Navigation.Public.Models.Buildings;

public class PositionDto
{
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public static PositionDto From(double latitude, double longitude) => new() { Latitude = latitude, Longitude = longitude };
}