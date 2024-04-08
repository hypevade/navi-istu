namespace Istu.Navigation.Infrastructure.EF.Filters;

public class EdgeFilter : BaseFilter
{
    public Guid? BuildingId { get; set; }
    public Guid? BuildingObjectId { get; set; }
    public int? FloorNumber { get; set; }
}