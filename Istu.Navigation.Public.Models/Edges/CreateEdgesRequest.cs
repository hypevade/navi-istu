using Istu.Navigation.Public.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models.Edges;

public class CreateEdgesRequest
{
    public required List<EdgeDto> Edges { get; set; }
}