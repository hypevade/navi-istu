namespace Istu.Navigation.Domain.Models.Entities;

public class EdgeEntity : BaseEntity
{
    public required Guid BuildingId { get; set; }
    public required Guid FromObject { get; set; }
    
    //TODO: костыль, не придумал, как хранить номер этажа ребра иначе
    public required int FloorNumber { get; set; }
    public required Guid ToObject { get; set; }
}