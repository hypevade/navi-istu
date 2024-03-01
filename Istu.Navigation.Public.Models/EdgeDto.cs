namespace Istu.Navigation.Public.Models;

public class EdgeDto
{
    public FloorObjectDto From { get; set; }
    public FloorObjectDto To { get; set; }
    public double Weight { get; set; }

    public EdgeDto(FloorObjectDto from, FloorObjectDto to, double weight)
    {
        From = from;
        To = to;
        Weight = weight; // Вес может представлять длину коридора, время перемещения и т.д.
    }
}