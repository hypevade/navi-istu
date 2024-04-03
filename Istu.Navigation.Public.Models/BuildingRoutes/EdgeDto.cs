using Istu.Navigation.Public.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models;

public class EdgeDto
{
    public BuildingObjectDto From { get; set; }
    public BuildingObjectDto To { get; set; }

    public EdgeDto(BuildingObjectDto from, BuildingObjectDto to, double weight)
    {
        From = from;
        To = to;
    }
}