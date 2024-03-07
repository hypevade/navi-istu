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

    public int FloorNumbers
    {
        get => floorNumbers;
        init
        {
            if (value < 0)
                throw new ArgumentException($"{nameof(FloorNumbers)} не может быть отрицательным",
                    nameof(FloorNumbers));
            floorNumbers = value;
        }
    }

    private readonly int floorNumbers;

    public string? Description { get; set; }

    public static BuildingDto ToDto(Building building) => new()
    {
        Id = building.Id,
        Title = building.Title,
        FloorNumbers = building.FloorNumbers,
        Description = building.Description
    };
}