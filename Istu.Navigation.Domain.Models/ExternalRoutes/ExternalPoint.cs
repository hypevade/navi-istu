namespace Istu.Navigation.Domain.Models.ExternalRoutes;

public class ExternalPoint(double latitude, double longitude)
{
    public double Latitude { get; set; } = latitude;
    public double Longitude { get; set; } = longitude;
}