namespace Istu.Navigation.Domain.Models.Entities;

public class FloorEntity
{
    public required Guid BuildingId { get; set; }
    public required Guid ImageId { get; set; }
    public required int FloorNumber { get; set; }
    public required bool IsDeleted { get; set; }
}