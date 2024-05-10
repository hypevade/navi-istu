namespace Istu.Navigation.Domain.Models.ExternalRoutes;

public class MapOptions
{
    public string FileName { get; set; } = string.Empty;
    public double MinLongitude { get; set; }
    public double MaxLongitude { get; set; }
    public double MinLatitude { get; set; }
    public double MaxLatitude { get; set; }
}