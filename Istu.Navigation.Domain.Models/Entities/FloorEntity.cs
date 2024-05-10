namespace Istu.Navigation.Domain.Models.Entities;

public class FloorEntity : BaseEntity
{
    public Guid BuildingId { get; set; }
    public int FloorNumber { get; set; }
}