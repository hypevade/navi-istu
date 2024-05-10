namespace Istu.Navigation.Public.Models.ExternalRoutes;

public class ExternalRouteResponse
{
    public TimeSpan TotalTime { get; set; }
    public List<ExternalPointDto> Points { get; set; } = new();
}