namespace Istu.Navigation.Domain.Models;

public class Building(string title, Guid id, List<Floor>? floors = null)
{
    public required string Title { get; init; } = title;
    public required Guid Id { get; init; } = id;

    public List<Floor>? Floors { get; set; } = floors;
}