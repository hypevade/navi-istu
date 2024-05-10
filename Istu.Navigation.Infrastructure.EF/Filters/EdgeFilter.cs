namespace Istu.Navigation.Infrastructure.EF.Filters;

public class EdgeFilter : BaseFilter
{
    public Guid? BuildingId { get; set; }
    public Guid? FromBuildingObjectId { get; set; }
    public Guid? ToBuildingObjectId { get; set; }
    public int? Floor { get; set; }
}