namespace Istu.Navigation.Domain.Models.InnerObjects;

public class Edge
{
    public InnerObject From { get; set; }
    public InnerObject To { get; set; }
    public double Weight { get; set; }

    public Edge(InnerObject from, InnerObject to, double weight)
    {
        From = from;
        To = to;
        Weight = weight;
    }
}