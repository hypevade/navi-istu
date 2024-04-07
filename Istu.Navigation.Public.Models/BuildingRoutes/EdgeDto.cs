namespace Istu.Navigation.Public.Models.BuildingRoutes;

public class EdgeDto
{
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }

    public EdgeDto(Guid fromId, Guid toId)
    {
        FromId = fromId;
        ToId = toId;
    }
}