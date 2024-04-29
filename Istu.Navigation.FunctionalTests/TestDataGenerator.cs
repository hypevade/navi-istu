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

    public static CreateFloorRequest GetCreateFloorRequest(string? link = null, int? floorNumber = null)
    {
        link ??= "TestLink";
        return new CreateFloorRequest
        {
            ImageLink = link,
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
                Title = $"TestBuilding_{i}",
                Description = $"TestDescription_{i}",
                Latitude = 0 + i,
                Longitude = 0 + i,
                Address = $"TestAddress_{i}"
            };
            requests.Add(request);
        }

        return requests;
    }
}