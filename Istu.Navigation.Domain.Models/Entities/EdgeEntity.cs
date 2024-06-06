namespace Istu.Navigation.Domain.Models.Entities;

public class EdgeEntity : BaseEntity
{
    public required Guid BuildingId { get; set; }
    public required Guid FromObject { get; set; }
    public required int FloorNumber { get; set; }
    public required Guid ToObject { get; set; }
}