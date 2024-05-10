using FluentAssertions;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Domain.Services.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;

namespace Istu.Navigation.UnitTests.BuildingRoutes;

public class RouteSearcherTests
{
    private readonly IRouteSearcher routeSearcher = new RouteSearcher();
    private readonly int testFloorNumber;

    public RouteSearcherTests()
    {
        testFloorNumber = 1;
    }


    [Test]
    public void Should_return_correct_route_when_two_obj()
    {
        var (objects, edges) = GetTwoObjectsWithEdge();

        var result = routeSearcher.CreateRoute(objects[0], objects[1], objects, edges);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.Count.Should().Be(2);
        result.Data.Should().BeEquivalentTo(objects);
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
            new(Guid.NewGuid(), objects[0], objects[1], testFloorNumber),
            new(Guid.NewGuid(), objects[1], objects[2], testFloorNumber),
            new(Guid.NewGuid(), objects[0], objects[2], testFloorNumber),
        };

        var result = routeSearcher.CreateRoute(objects[0], objects[2], objects, edges);
        
        result.IsSuccess.Should().BeTrue();
        var path = result.Data;
        path.Should().NotBeNull();

        path.Count.Should().Be(2);
        path[0].Should().BeEquivalentTo(objects[0]);
        path[1].Should().BeEquivalentTo(objects[2]);
    }

    [Test]
    public void Should_return_error_when_source_and_target_are_the_same()
    {
        var (objects, edges) = GetTwoObjectsWithEdge();

        var result = routeSearcher.CreateRoute(objects[0], objects[0], objects, edges);
        result.IsFailure.Should().BeTrue();
        result.Data.Should().BeNull();

        result.ApiError.Urn.Should().BeEquivalentTo(
            BuildingsErrors.TargetObjectIsEqualToSourceError(objects[0].Id).Urn);
    }
    
    [Test]
    public void Should_return_internal_error_when_no_edges()
    {
        var (objects, _) = GetTwoObjectsWithEdge();

        var result = routeSearcher.CreateRoute(objects[0], objects[1], objects, []);
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();

        result.ApiError.Should().NotBeNull();
        result.ApiError.Urn.Should()
            .BeEquivalentTo(CommonErrors.InternalServerError().Urn);
    }
    
    [Test]
    public void Should_return_internal_error_when_empty_collection_objects()
    {
        var (objects, _) = GetTwoObjectsWithEdge();

        var result = routeSearcher.CreateRoute(objects[0], objects[1], [], []);
        result.IsSuccess.Should().BeFalse();
        result.ApiError.Should().NotBeNull();
        result.ApiError.Urn.Should().Be(CommonErrors.InternalServerError().Urn);
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
            new(Guid.NewGuid(), objects[0], objects[1], testFloorNumber),
        };

        var result = routeSearcher.CreateRoute(objects[0], objects[2], objects, edges);
        result.IsSuccess.Should().BeFalse();
        result.ApiError.Should().NotBeNull();
        result.ApiError.Urn.Should()
            .Be(BuildingsErrors.BuildingRouteNotFoundError(objects[0].Id, objects[1].Id).Urn);
    }

    private static (List<BuildingObject> objects, List<Edge> edges) GetTwoObjectsWithEdge()
    {
        var points = new List<(double X, double Y)>
        {
            (0, 0),
            (1, 1),
        };

        var objects = GenerateBuildingObjects(points);
        var edges = new List<Edge>
        {
            new(Guid.NewGuid(), objects[0], objects[1], 1)
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