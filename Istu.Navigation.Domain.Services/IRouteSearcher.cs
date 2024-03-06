using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using QuikGraph;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;

namespace Istu.Navigation.Domain.Services;

public interface IRouteSearcher
{
    public OperationResult<FloorRoute> CreateRoute(BuildingObject fromBuildingObject, BuildingObject toBuildingObject, Floor floor);
}

public class RouteSearcher : IRouteSearcher
{
    private readonly Func<Edge<BuildingObject>, double> edgeWeightFunc = edge =>
        Math.Sqrt((edge.Source.X - edge.Target.X) * (edge.Source.X - edge.Target.X) +
                  (edge.Source.Y - edge.Target.Y) * (edge.Source.Y - edge.Target.Y));
    
    public OperationResult<FloorRoute> CreateRoute(BuildingObject fromBuildingObject, BuildingObject toBuildingObject, Floor floor)
    {
        if (floor.Objects.Count == 0)
            return OperationResult<FloorRoute>.Failure(
                BuildingRoutesErrors.FloorContainsNoObjectsError(floor.BuildingId, floor.Number));
        
        if (floor.Edges.Count == 0)
            return OperationResult<FloorRoute>.Failure(
                BuildingRoutesErrors.FloorContainsNoEdgesError(floor.BuildingId, floor.Number));

        if (fromBuildingObject.Id == toBuildingObject.Id)
            return OperationResult<FloorRoute>.Failure(BuildingRoutesErrors.TargetObjectIsEqualToSourceError(fromBuildingObject.Id));

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
            return OperationResult<FloorRoute>.Failure(BuildingRoutesErrors.BuildingRouteNotFoundError(fromBuildingObject.Id, toBuildingObject.Id));
        
        var shortestPath = new List<BuildingObject> { fromBuildingObject };
        
        foreach (var edge in path)
            shortestPath.Add(edge.Target);
        
        var floorRoute =  new FloorRoute
        {
            StartObject = fromBuildingObject,
            FinishObject = toBuildingObject,
            Objects = shortestPath
        };
        
        return OperationResult<FloorRoute>.Success(floorRoute);
    }
}
