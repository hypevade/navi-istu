using Istu.Navigation.Domain.Models.InnerObjects;
using QuikGraph;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;

namespace Istu.Navigation.UnitTests;

public class Tests
{
   

    [Test]
    public void Test1()
    {
        var graph = new AdjacencyGraph<BuildingObject, Edge<BuildingObject>>(true);
        var bO = new BuildingObject()
        {
            Id = Guid.NewGuid(),
            Title = "test0",
            Type = BuildingObjectType.Toilet,
            BuildingId = Guid.NewGuid(),
            Floor = 1,
            X = 0,
            Y = 0
        };
        
        var b1 = new BuildingObject()
        {
            Id = Guid.NewGuid(),
            Title = "test 1 0.5",
            Type = BuildingObjectType.Toilet,
            BuildingId = Guid.NewGuid(),
            Floor = 1,
            X = 1,
            Y = 0.5
        };
        
        var b2 = new BuildingObject()
        {
            Id = Guid.NewGuid(),
            Title = "test 1",
            Type = BuildingObjectType.Toilet,
            BuildingId = Guid.NewGuid(),
            Floor = 1,
            X = 1,
            Y = 1
        };
        
        var b3 = new BuildingObject()
        {
            Id = Guid.NewGuid(),
            Title = "test 2",
            Type = BuildingObjectType.Toilet,
            BuildingId = Guid.NewGuid(),
            Floor = 1,
            X = 2,
            Y = 2
        };
        graph.AddVertex(bO);
        graph.AddVertex(b1);
        graph.AddVertex(b2);
        graph.AddVertex(b3);

// Create the edges
        var bOToB1 = new Edge<BuildingObject>(bO, b1);
        var bOToB2 = new Edge<BuildingObject>(bO, b2);
        var b1ToB2 = new Edge<BuildingObject>(b1, b2);
        var b2ToB3 = new Edge<BuildingObject>(b2, b3);


// Add the edges
        graph.AddEdge(bOToB1);
        //graph.AddEdge(bOToB2);
        graph.AddEdge(b1ToB2);
        graph.AddEdge(b2ToB3);

        var dijkstra =
            new DijkstraShortestPathAlgorithm<BuildingObject, Edge<BuildingObject>>(graph,
                x => Math.Sqrt((x.Source.X - x.Target.X) * (x.Source.X - x.Target.X) +
                               (x.Source.Y - x.Target.Y) * (x.Source.Y - x.Target.Y)));
        

// attach a distance observer to give us the shortest path distances
        var distObserver =
            new VertexDistanceRecorderObserver<BuildingObject, Edge<BuildingObject>>(x =>
                Math.Sqrt((x.Source.X - x.Target.X) * (x.Source.X - x.Target.X) +
                          (x.Source.Y - x.Target.Y) * (x.Source.Y - x.Target.Y)));
        distObserver.Attach(dijkstra);

// Attach a Vertex Predecessor Recorder Observer to give us the paths
        var predecessorObserver =
            new VertexPredecessorRecorderObserver<BuildingObject, Edge<BuildingObject>>();
        predecessorObserver.Attach(dijkstra);

// Run the algorithm with A set to be the source
        dijkstra.Compute(bO);

        foreach (var kvp in distObserver.Distances)
            Console.WriteLine("Distance from root to node {0} is {1}", kvp.Key.Title, kvp.Value);

        /*foreach (var kvp in predecessorObserver.VerticesPredecessors)
            Console.WriteLine("If you want to get to {0} you have to enter through the in edge {1}", kvp.Key.Title,
                kvp.Value.Source.Title);*/
        foreach (var kvp  in predecessorObserver.VerticesPredecessors)
        {
            Console.WriteLine(kvp.Key.Title);
        }

        if (predecessorObserver.TryGetPath(b3, out IEnumerable<Edge<BuildingObject>> path)) // Пытаемся получить путь до вершины 4
        {
            foreach (var edge in path)
            {
                Console.WriteLine($"{edge.Source.Title} -> {edge.Target.Title}");
            }
        }
        else
        {
            Console.WriteLine("Путь не найден.");
        }
    }
}