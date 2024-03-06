using FluentAssertions;
using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.UnitTests;

public class RouteSearcherTests
{
    private readonly IRouteSearcher routeSearcher = new RouteSearcher();

    [Test]
    public void Should_return_correct_route_when_two_obj()
    {
        var (objects, edges) = GetObjectsAndEdges();

        var floor = new Floor
        {
            BuildingId = Guid.Empty,
            Number = 0,
            Objects = objects,
            Edges = edges,
            ImageLink = "test"
        };

        var result = routeSearcher.CreateRoute(objects[0], objects[1], floor);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.Objects.Count.Should().Be(2);
        result.Data.Objects.Should().BeEquivalentTo(objects);
    }
    
    [Test]
    public void Should_return_shortest_route_when_two_routes_exist()
    {
        var points = new List<(double X, double Y)>
        {
            (0, 0),
            (1, 0),
            (1, 1)
        };

        var objects = GenerateBuildingObjects(points);
        var edges = new List<Edge>
        {
            new(objects[0], objects[1]),
            new(objects[1], objects[2]),
            new(objects[0], objects[2]),
        };

        var floor = new Floor
        {
            BuildingId = Guid.Empty,
            Number = 0,
            Objects = objects,
            Edges = edges,
            ImageLink = "test"
        };

        var result = routeSearcher.CreateRoute(objects[0], objects[2], floor);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.Objects.Count.Should().Be(2);
        result.Data.Objects[0].Should().BeEquivalentTo(objects[0]);
        result.Data.Objects[1].Should().BeEquivalentTo(objects[2]);
    }
    [Test]
    public void Should_return_error_when_source_and_target_are_the_same()
    {
        var (objects, edges) = GetObjectsAndEdges();

        var floor = new Floor
        {
            BuildingId = Guid.Empty,
            Number = 0,
            Objects = objects,
            Edges = edges,
            ImageLink = "test"
        };

        var result = routeSearcher.CreateRoute(objects[0], objects[0], floor);
        result.IsFailure.Should().BeTrue();
        result.Data.Should().BeNull();

        result.ApiError.Urn.Should()
            .BeEquivalentTo(BuildingRoutesErrors.TargetObjectIsEqualToSourceError(objects[0].Id).Urn);
        result.Data.Objects.Should().BeEquivalentTo(objects);
    }
    
    [Test]
    public void Should_return_error_when_no_edges()
    {
        var (objects, _) = GetObjectsAndEdges();

        var floor = new Floor
        {
            BuildingId = Guid.Empty,
            Number = 0,
            Objects = objects,
            Edges = new List<Edge>(),
            ImageLink = "test"
        };

        var result = routeSearcher.CreateRoute(objects[0], objects[1], floor);
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();

        result.ApiError.Should().NotBeNull();
        result.ApiError.Urn.Should()
            .BeEquivalentTo(BuildingRoutesErrors.BuildingRouteNotFoundError(objects[0].Id, objects[1].Id).Urn);
    }
    
    [Test]
    public void Should_return_error_when_empty_floor()
    {
        var (objects, _) = GetObjectsAndEdges();
        
        var floor = new Floor
        {
            BuildingId = Guid.Empty,
            Number = 0,
            Objects = [],
            Edges = [],
            ImageLink = "test"
        };

        var result = routeSearcher.CreateRoute(objects[0], objects[1], floor);
        result.IsSuccess.Should().BeFalse();
        result.ApiError.Should().NotBeNull();
        result.ApiError.Urn.Should().Be(BuildingRoutesErrors.BuildingRouteNotFoundError(objects[0].Id, objects[1].Id).Urn);
    }
    
    [Test]
    public void Should_return_not_found_when_no_route_between_obj()
    {
        var points = new List<(double X, double Y)>
        {
            (0, 0),
            (1, 1),
            (2, 2),
        };

        var objects = GenerateBuildingObjects(points);
        var edges = new List<Edge>()
        {
            new(objects[0], objects[1]),
        };

        var floor = new Floor
        {
            BuildingId = Guid.Empty,
            Number = 0,
            Objects = objects,
            Edges = edges,
            ImageLink = "test"
        };

        var result = routeSearcher.CreateRoute(objects[0], objects[2], floor);
        result.IsSuccess.Should().BeFalse();
        result.ApiError.Should().NotBeNull();
        result.ApiError.Urn.Should().Be(BuildingRoutesErrors.BuildingRouteNotFoundError(objects[0].Id, objects[1].Id).Urn);
    }

    private static (List<BuildingObject> objects, List<Edge> edges) GetObjectsAndEdges()
    {
        var points = new List<(double X, double Y)>
        {
            (0, 0),
            (1, 1),
        };

        var objects = GenerateBuildingObjects(points);
        var edges = new List<Edge>
        {
            new(objects[0], objects[1]),
        };
        return (objects, edges);
    }

    private static List<BuildingObject> GenerateBuildingObjects(List<(double X, double Y)> points)
    {
        var objects = new List<BuildingObject>();
        var buildingId = Guid.NewGuid();

        for (var index = 0; index < points.Count; index++)
        {
            var point = points[index];
            objects.Add(new BuildingObject
            {
                Id = Guid.NewGuid(),
                Title = $"Object {index}",
                Floor = 1,
                BuildingId = buildingId,
                Type = BuildingObjectType.Node,
                X = point.X,
                Y = point.Y
            });
        }

        return objects;
    }
}