namespace Istu.Navigation.Infrastructure.EF.Filters;

public class BuildingObjectFilter : BaseFilter
{
    public Guid? Id { get; set; }
    public Guid? BuildingId { get; set; }
    public int? Floor { get; set; }
    public string? Title { get; set; }
    public HashSet<string>? Types { get; set; }
}