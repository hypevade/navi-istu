namespace Istu.Navigation.Domain.Models.ExternalRoutes;

public class ExternalRoute(List<ExternalPoint> points, TimeSpan totalTime, ExternalRouteType type)
{
    public ExternalRouteType Type { get; set; }
    public List<ExternalPoint> Points { get; set; } = points;
    public TimeSpan TotalTime { get; set; } = totalTime;
}