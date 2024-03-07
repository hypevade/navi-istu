using FluentAssertions;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.UnitTests;

public class RouteSearcherTests
{
    private readonly IRouteSearcher routeSearcher = new RouteSearcher();
    private readonly ImageLink testImageLink;
    private readonly Guid floorId;

    public RouteSearcherTests()
    {
        floorId = Guid.NewGuid();
        testImageLink = new ImageLink(Guid.NewGuid(), floorId, "test");
    }
    

    [Test]
    public void Should_return_correct_route_when_two_obj()
    {
        var (objects, edges) = GetObjectsAndEdges();

        var floor = new Floor(id: floorId, number: 1, objects: objects, edges: edges, buildingId: Guid.NewGuid(),
            imageLink: testImageLink);

        var result = routeSearcher.CreateRoute(objects[0], objects[1], floor);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.Route.Count.Should().Be(2);
        result.Data.Route.Should().BeEquivalentTo(objects);
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
            new(Guid.NewGuid(), objects[0], objects[1]),
            new(Guid.NewGuid(), objects[1], objects[2]),
            new(Guid.NewGuid(), objects[0], objects[2]),
        };
        
        var floor = new Floor(id: floorId, number: 1, objects: objects, edges: edges, buildingId: Guid.NewGuid(),
            imageLink: testImageLink);

        var result = routeSearcher.CreateRoute(objects[0], objects[2], floor);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.Route.Count.Should().Be(2);
        result.Data.Route[0].Should().BeEquivalentTo(objects[0]);
        result.Data.Route[1].Should().BeEquivalentTo(objects[2]);
    }
    [Test]
    public void Should_return_error_when_source_and_target_are_the_same()
    {
        var (objects, edges) = GetObjectsAndEdges();
    
        var floor = new Floor(
            id: floorId,
            number: 1,
            objects: objects,
            edges: edges,
            buildingId: Guid.NewGuid(),
            imageLink: testImageLink
        );

        var result = routeSearcher.CreateRoute(objects[0], objects[0], floor);
        result.IsFailure.Should().BeTrue();
        result.Data.Should().BeNull();

        result.ApiError.Urn.Should().BeEquivalentTo(
            BuildingRoutesErrors.TargetObjectIsEqualToSourceError(objects[0].Id).Urn);
    }
    
    [Test]
    public void Should_return_error_when_no_edges()
    {
        var (objects, _) = GetObjectsAndEdges();

        var floor = new Floor(id: floorId, number: 1, objects: objects, edges: new List<Edge>(), buildingId: Guid.NewGuid(),
            imageLink: testImageLink);

        var result = routeSearcher.CreateRoute(objects[0], objects[1], floor);
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();

        result.ApiError.Should().NotBeNull();
        result.ApiError.Urn.Should()
            .BeEquivalentTo(BuildingRoutesErrors.FloorContainsNoEdgesError(floor.BuildingId, floor.Number).Urn);
    }
    
    [Test]
    public void Should_return_error_when_empty_floor()
    {
        var (objects, _) = GetObjectsAndEdges();
        
        var floor = new Floor(id: floorId, number: 1, objects: [], edges: [], buildingId: Guid.NewGuid(),
            imageLink: testImageLink);

        var result = routeSearcher.CreateRoute(objects[0], objects[1], floor);
        result.IsSuccess.Should().BeFalse();
        result.ApiError.Should().NotBeNull();
        result.ApiError.Urn.Should().Be(BuildingRoutesErrors.FloorContainsNoObjectsError(floor.BuildingId, floor.Number).Urn);
    }
    
    [Test]
    public void Should_return_not_found_when_no_route_between_obj()
    {
        var points = new List<(double X, double Y)>
        {
            (0, 0),
            (0.5, 0.5),
            (1, 1),
        };

        var objects = GenerateBuildingObjects(points);
        var edges = new List<Edge>()
        {
            new(Guid.NewGuid(), objects[0], objects[1]),
        };

        var floor = new Floor(id: floorId, number: 1, objects: objects, edges: edges, buildingId: Guid.NewGuid(),
            imageLink: testImageLink);

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
            new(Guid.NewGuid(), objects[0], objects[1])
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
            objects.Add(new BuildingObject(id: Guid.NewGuid(), buildingId: buildingId, title: $"Object {index}",
                floor: 1, type: BuildingObjectType.Node, x: point.X, y: point.Y));

        }

        return objects;
    }
}