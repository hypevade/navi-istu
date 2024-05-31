using Istu.Navigation.Domain.Models.BuildingRoutes;

namespace Istu.Navigation.Infrastructure.EF.Filters;

public class BuildingObjectFilter : BaseFilter
{
    public Guid? BuildingObjectId { get; set; }
    public Guid? BuildingId { get; set; }
    public int? Floor { get; set; }
    public string? Title { get; set; }
    public BuildingObjectType? Type { get; set; }
}