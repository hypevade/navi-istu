using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using QuikGraph;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;

namespace Istu.Navigation.Domain.Services.BuildingRoutes;

public interface IRouteSearcher
{
    public OperationResult<List<BuildingObject>> CreateRoute(BuildingObject fromBuildingObject, BuildingObject toBuildingObject, List<BuildingObject> objects, List<Edge> edges);
}

public class RouteSearcher : IRouteSearcher
{
    private readonly Func<Edge<BuildingObject>, double> edgeWeightFunc = edge =>
        Math.Sqrt((edge.Source.X - edge.Target.X) * (edge.Source.X - edge.Target.X) +
                  (edge.Source.Y - edge.Target.Y) * (edge.Source.Y - edge.Target.Y));

    public OperationResult<List<BuildingObject>> CreateRoute(BuildingObject fromBuildingObject,
        BuildingObject toBuildingObject, List<BuildingObject> objects, List<Edge> edges)
    {
        var validationResult = ValidateInput(objects, edges, fromBuildingObject, toBuildingObject);
        if (validationResult.IsFailure)
            return OperationResult<List<BuildingObject>>.Failure(validationResult.ApiError);

        var floorGraph = BuildGraph(objects, edges);

        var pathResult = FindShortestPath(floorGraph, fromBuildingObject, toBuildingObject);
        if (pathResult.IsFailure)
            return OperationResult<List<BuildingObject>>.Failure(pathResult.ApiError);

        var path = new List<BuildingObject> { fromBuildingObject };
        foreach (var edge in pathResult.Data)
            path.Add(edge.Target);
        
        return OperationResult<List<BuildingObject>>.Success(path);
    }

    private OperationResult ValidateInput(List<BuildingObject> objects, List<Edge> edges, BuildingObject from,
        BuildingObject to)
    {
        if (objects.Count == 0 || edges.Count == 0)
            return OperationResult.Failure(CommonErrors.InternalServerError());

        return from.Id == to.Id
            ? OperationResult.Failure(BuildingsErrors.TargetObjectIsEqualToSourceError(from.Id))
            : OperationResult.Success();
    }

    private AdjacencyGraph<BuildingObject, Edge<BuildingObject>> BuildGraph(List<BuildingObject> objects,
        List<Edge> edges)
    {
        var graph = new AdjacencyGraph<BuildingObject, Edge<BuildingObject>>();

        objects.ForEach(o => graph.AddVertex(o));
        edges.ForEach(edge => graph.AddEdge(new Edge<BuildingObject>(edge.From, edge.To)));

        return graph;
    }

    private OperationResult<IEnumerable<Edge<BuildingObject>>> FindShortestPath(
        AdjacencyGraph<BuildingObject, Edge<BuildingObject>> graph, BuildingObject from, BuildingObject to)
    {
        var pathAlgorithm =
            new DijkstraShortestPathAlgorithm<BuildingObject, Edge<BuildingObject>>(graph, edgeWeightFunc);
        var predecessorObserver = new VertexPredecessorRecorderObserver<BuildingObject, Edge<BuildingObject>>();
        predecessorObserver.Attach(pathAlgorithm);

        pathAlgorithm.Compute(from);

        if (!predecessorObserver.TryGetPath(to, out var path))
            return OperationResult<IEnumerable<Edge<BuildingObject>>>.Failure(
                BuildingsErrors.BuildingRouteNotFoundError(from.Id, to.Id));
        

        return OperationResult<IEnumerable<Edge<BuildingObject>>>.Success(path);
    }
}
