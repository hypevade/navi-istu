using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class Building
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public int FloorNumbers => Floors.Count;

    public required List<FloorInfo> Floors { get; set; }

    public string? Description { get; set; }

    public static BuildingEntity ToEntity(Building building) => new()
    {
        Id = building.Id,
        Title = building.Title,
        FloorNumbers = building.FloorNumbers,
        Description = building.Description,
        IsDeleted = false
    };
}