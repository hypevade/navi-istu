using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Domain.Repositories;
using QuikGraph;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;

namespace Istu.Navigation.Domain.Services;

public interface IRouteSearcher
{
    public FloorRoute CreateRoute(BuildingObject fromBuildingObject, BuildingObject toBuildingObject, Floor floor);
}

public class RouteSearcher : IRouteSearcher
{
    private readonly Func<Edge<BuildingObject>, double> edgeWeightFunc = edge =>
        Math.Sqrt((edge.Source.X - edge.Target.X) * (edge.Source.X - edge.Target.X) +
                  (edge.Source.Y - edge.Target.Y) * (edge.Source.Y - edge.Target.Y));

    public FloorRoute CreateRoute(BuildingObject fromBuildingObject, BuildingObject toBuildingObject, Floor floor)
    {
        var floorGraph = new AdjacencyGraph<BuildingObject, Edge<BuildingObject>>();
        floor.Objects
            .ForEach(bO => { floorGraph.AddVertex(bO); });

        floor.Edges.ForEach(bO => { floorGraph.AddEdge(new Edge<BuildingObject>(bO.From, bO.To)); });

        var pathAlgorithm =
            new DijkstraShortestPathAlgorithm<BuildingObject, Edge<BuildingObject>>(floorGraph, edgeWeightFunc);
        var predecessorObserver =
            new VertexPredecessorRecorderObserver<BuildingObject, Edge<BuildingObject>>();
        predecessorObserver.Attach(pathAlgorithm);

        pathAlgorithm.Compute(fromBuildingObject);
        
        if (!predecessorObserver.TryGetPath(toBuildingObject, out var path))
            throw new Exception();
        
        var shortestPath = new List<BuildingObject> { fromBuildingObject };
        foreach (var edge in path)
            shortestPath.Add(edge.Target);

        return new FloorRoute
        {
            StartObject = fromBuildingObject,
            FinishObject = toBuildingObject,
            Objects = shortestPath
        };

    }
}
