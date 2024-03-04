using FluentAssertions;
using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Domain.Services;

namespace Istu.Navigation.UnitTests;

public class RouteSearcherTests
{
    private IRouteSearcher routeSearcher = new RouteSearcher();

    [Test]
    public void Should_return_shortest_route()
    {
        var objects = GenerateBuildingObjects();
        var edges = new List<Edge>()
        {
            new Edge(objects[0], objects[1]),
            new Edge(objects[1], objects[2]),
            new Edge(objects[2], objects[3]),
            new Edge(objects[3], objects[4]),
            new Edge(objects[0], objects[4]),
        };
        
        var floor = new Floor
        {
            BuildingId = Guid.Empty,
            Number = 0,
            Objects = objects,
            Edges = edges,
            ImageLink = "test"
        };

        var result = routeSearcher.CreateRouteOR(objects[0], objects[4], floor);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        Console.WriteLine(result.Data!.Objects.Count);
        result.Data.Objects.ForEach(x=>Console.WriteLine(x.Title));
    }

    private static List<BuildingObject> GenerateBuildingObjects()
    {
        var objects = new List<BuildingObject>
        {
            new BuildingObject
            {
                Id = Guid.NewGuid(),
                Title = "Object 1",
                Floor = 1,
                BuildingId = Guid.NewGuid(),
                Type = BuildingObjectType.Node,
                X = 10.0,
                Y = 20.0
            },
            new BuildingObject
            {
                Id = Guid.NewGuid(),
                Title = "Object 2",
                Floor = 2,
                BuildingId = Guid.NewGuid(),
                Type = BuildingObjectType.Node,
                X = 20.0,
                Y = 40.0
            },
            new BuildingObject
            {
                Id = Guid.NewGuid(),
                Title = "Object 3",
                Floor = 3,
                BuildingId = Guid.NewGuid(),
                Type = BuildingObjectType.Entrance,
                X = 30.0,
                Y = 60.0
            },
            new BuildingObject
            {
                Id = Guid.NewGuid(),
                Title = "Object 4",
                Floor = 4,
                BuildingId = Guid.NewGuid(),
                Type = BuildingObjectType.Node,
                X = 40.0,
                Y = 80.0
            },
            new BuildingObject
            {
                Id = Guid.NewGuid(),
                Title = "Object 5",
                Floor = 5,
                BuildingId = Guid.NewGuid(),
                Type = BuildingObjectType.Node,
                X = 50.0,
                Y = 100.0
            }
        };

        return objects;
    }
}