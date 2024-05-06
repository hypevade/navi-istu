namespace Istu.Navigation.Domain.Models.ExternalRoutes;

public class ExternalPoint(double latitude, double longitude, string? name = null)
{
    public required double Latitude { get; set; } = latitude;
    public required double Longitude { get; set; } = longitude;
    public string? Name { get; set; } = name;
}