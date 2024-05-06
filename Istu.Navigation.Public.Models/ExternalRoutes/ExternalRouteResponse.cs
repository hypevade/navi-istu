namespace Istu.Navigation.Public.Models.ExternalRoutes;

public class ExternalRouteResponse
{
    public TimeSpan TotalTime { get; set; }
    public List<ExternalRoutePoint> Points { get; set; } = new();
}