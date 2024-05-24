using System.Text;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.Buildings;
using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.FunctionalTests;

public static class TestDataGenerator
{
    public static async Task<List<Guid>> CreateRandomBuildings(IBuildingsClient client, int count)
    {
        var requests = GetCreateBuildingRequests(count);
        var buildingIds = new List<Guid>();
        foreach (var request in requests)
        {
            var buildingId = await client.CreateBuildingAsync(request).ConfigureAwait(false);
            buildingIds.Add(buildingId.Data.BuildingId);
        }

        return buildingIds;
    }

    public static async Task<List<Guid>> CreateRandomBuildingsObjects(IBuildingObjectsClient client, Guid buildingId,
        int count)
    {
        var requests = GenerateBuildingObjects(count, buildingId);
        var boIds = new List<Guid>();
        foreach (var request in requests)
        {
            var create = await client.CreateBuildingObjectAsync(request).ConfigureAwait(false);
            boIds.Add(create.Data.BuildingObjectId);
        }

        return boIds;
    }



    public static CreateFloorRequest GetCreateFloorRequest(int? floorNumber = null)
    {
        return new CreateFloorRequest
        {
            FloorNumber = floorNumber
        };
    }

    public static List<CreateBuildingRequest> GetCreateBuildingRequests(int count)
    {
        var requests = new List<CreateBuildingRequest>();
        for (var i = 0; i < count; i++)
        {
            var request = new CreateBuildingRequest
            {
                Title = $"TestBuilding_{i}_({GetRandomString(5)})",
                Description = $"TestDescription_{i}",
                Position = PositionDto.From(0 + i, 0 + i),
                Address = $"TestAddress_{i}"
            };
            requests.Add(request);
        }

        return requests;
    }

    public static List<CreateBuildingObjectRequest> GenerateBuildingObjects(int count, Guid buildingId)
    {
        var random = new Random();
        var points = new List<(double X, double Y)>();
        for (var i = 0; i < count; i++)
        {
            points.Add((random.NextDouble(), random.NextDouble()));
        }

        return GenerateBuildingObjects(points, buildingId);
    }

    public static List<CreateBuildingObjectRequest> GenerateBuildingObjects(List<(double X, double Y)> points,
        Guid buildingId)
    {

        return points.Select((point, i) => new CreateBuildingObjectRequest()
            {
                Title = $"Object {i}({GetRandomString(2)})",
                BuildingId = buildingId,
                Floor = 1,
                Type = BuildingObjectType.Node,
                X = point.X,
                Y = point.Y
            })
            .ToList();
    }

    public static string GetRandomString(int length)
    {
        if (length < 1) return "";
        var r = new Random();
        var s = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            char a = (char)r.Next(0, 255);
            s.Append(a);
        }

        return s.ToString();
    }
}