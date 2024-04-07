using FluentAssertions;
using Istu.Navigation.Public.Models;
using Istu.Navigation.TestClient;
using System.Net;

namespace Istu.Navigation.FunctionalTests;

public class BuildingsServiceTests
{
    private IBuildingsClient client = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        var istuNavigationTestClient = await WebHostInstaller.GetHttpClient().ConfigureAwait(false);
        client = istuNavigationTestClient.BuildingsClient;
    }

    [Test]
    public async Task Should_create_building_when_request_is_valid()
    {
        var testBuilding = new CreateBuildingRequest()
        {
            Title = "TestBuilding",
            FloorNumbers = 5,
            Description = "TestDescription"
        };
        
        var createBuildingResponse = await client.CreateBuildingAsync(testBuilding);
        var buildingId = createBuildingResponse.BuildingId;
        var building = await client.GetBuildingByIdAsync(buildingId);
        
        building.Should().NotBeNull();
        building.Title.Should().Be(testBuilding.Title);
        building.Description.Should().Be(testBuilding.Description);
        building.Id.Should().Be(buildingId); 
    }
    
    [Test]
    public void Should_return_404_when_building_does_not_exist()
    {
        var ex = Assert.ThrowsAsync<HttpRequestException>(async () => await client.GetBuildingByIdAsync(Guid.NewGuid()));
        ex.Should().NotBeNull();
        ex?.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    
    
}