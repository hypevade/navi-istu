using FluentAssertions;
using Istu.Navigation.TestClient;
using System.Net;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Public.Models.Buildings;
using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.FunctionalTests;

public class BuildingsServiceTests
{
    private IBuildingsClient client = null!;
    private BuildingsDbContext dbContext = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        var istuNavigationTestClient = await WebHostInstaller.GetHttpClient().ConfigureAwait(false);
        client = istuNavigationTestClient.Client.BuildingsClient;
        dbContext = istuNavigationTestClient.DbContext;
    }

    [Test]
    public void METHOD()
    {
        Console.WriteLine("Ввведите первое условие: Если человек занимается йогой");
        Console.WriteLine("Ввведите второе условие (необязательно): ");
        Console.WriteLine("Он становится гибче");
    }

    [Test]
    public async Task Create_Should_create_building_when_request_is_valid()
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
    public void GetBuildingById_Should_return_404_when_building_does_not_exist()
    {
        var ex = Assert.ThrowsAsync<HttpRequestException>(async () => await client.GetBuildingByIdAsync(Guid.NewGuid()));
        ex.Should().NotBeNull();
        ex?.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task Get_All_Should_return_all_buildings()
    {
        var buildingEntity  = new BuildingEntity()
        {
            Id = Guid.NewGuid(),
            Title = "TestBuilding",
            FloorNumbers = 5,
            Description = "TestDescription",
            IsDeleted = false
        };
        
        await dbContext.Buildings.AddAsync(buildingEntity).ConfigureAwait(false);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        var buildings = await client.GetAllAsync();
        buildings.Should().HaveCount(1);
        buildings.First().Id.Should().Be(buildingEntity.Id);
        buildings.First().Title.Should().Be(buildingEntity.Title);
        buildings.First().FloorNumbers.Should().Be(buildingEntity.FloorNumbers);
        buildings.First().Description.Should().Be(buildingEntity.Description);
    }
}