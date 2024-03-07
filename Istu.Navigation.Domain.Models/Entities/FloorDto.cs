namespace Istu.Navigation.Domain.Models.Entities;

public class FloorDto
{
    public required Guid Id { get; set; }
    public required Guid BuildingId { get; set; }
    public required Guid ImageId { get; set; }
    public required int Number { get; set; }
}