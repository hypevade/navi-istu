using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.Buildings;
using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.FunctionalTests;

public static class TestDataGenerator
{
    public static async Task<List<Guid>> CreateRandomBuildings(IBuildingsClient client, int count)
    {
        var requests = GetCreateRequests(count);
        var buildingIds = new List<Guid>();
        foreach (var request in requests)
        {
            var buildingId = await client.CreateBuildingAsync(request).ConfigureAwait(false);
            buildingIds.Add(buildingId.Data.BuildingId);
        }

        return buildingIds;
    }

    public static List<CreateBuildingRequest> GetCreateRequests(int count)
    {
        var floors = GetTestFloors(new List<int> { 1, 2, 3, 4 });
        var requests = new List<CreateBuildingRequest>();
        for (var i = 0; i < count; i++)
        {
            var request = new CreateBuildingRequest
            {
                Title = $"TestBuilding_{i}",
                Description = $"TestDescription_{i}",
                Floors = floors
            };
            requests.Add(request);
        }

        return requests;
    }

    public static List<CreateFloorDto> GetTestFloors(IEnumerable<int> floorNumbers)
    {
        return floorNumbers.Select(x => new CreateFloorDto { FloorNumber = x, ImageLink = "TestLink_" + x }).ToList();
    }
}