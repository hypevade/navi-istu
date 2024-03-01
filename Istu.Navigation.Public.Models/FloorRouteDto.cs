namespace Istu.Navigation.Public.Models;

public class FloorRouteDto
{
    public required FloorObjectDto StartObject { get; set; }
    public required FloorObjectDto FinishObject { get; set; }
    public required List<FloorObjectDto> Objects { get; set; }
}