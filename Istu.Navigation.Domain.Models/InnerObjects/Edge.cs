namespace Istu.Navigation.Domain.Models.InnerObjects;

public class Edge
{
    public BuildingObject From { get; set; }
    public BuildingObject To { get; set; }
    public double Weight { get; set; }

    public Edge(BuildingObject from, BuildingObject to, double weight)
    {
        From = from;
        To = to;
        Weight = weight;
    }
}