using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class Building
{
    public Building(Guid id, string title, int floorNumbers)
    {
        Id = id;
        Title = title;
        FloorNumbers = floorNumbers;
    }

    public Guid Id { get; init; }
    public string Title { get; set; }
    public int FloorNumbers{ get; set; }

    public string? Description { get; set; }

    public static BuildingEntity ToDto(Building building) => new()
    {
        Id = building.Id,
        Title = building.Title,
        FloorNumbers = building.FloorNumbers,
        Description = building.Description,
        IsDeleted = false
    };
}