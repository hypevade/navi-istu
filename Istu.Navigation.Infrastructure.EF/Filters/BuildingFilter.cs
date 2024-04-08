namespace Istu.Navigation.Infrastructure.EF.Filters;

public class BuildingFilter : BaseFilter
{
    public Guid? BuildingId { get; set; }
    public string? Title { get; set; }
    public int? FloorNumber { get; set; }
}