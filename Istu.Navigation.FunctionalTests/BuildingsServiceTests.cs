using FluentAssertions;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using Istu.Navigation.Public.Models.Buildings;
using Istu.Navigation.TestClient;
using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.FunctionalTests;

public class BuildingsServiceTests
{
    private IBuildingsClient client = null!;
    private BuildingsDbContext dbContext = null!;
    

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var istuNavigationTestClient = await WebHostInstaller.GetHttpClient().ConfigureAwait(false);
        client = istuNavigationTestClient.Client.BuildingsClient;
        dbContext = istuNavigationTestClient.DbContext;
    }
    
    [TearDown]
    public async Task TearDown()
    {
        dbContext.Buildings.RemoveRange(dbContext.Buildings);
        dbContext.ImageLinks.RemoveRange(dbContext.ImageLinks);
        await dbContext.SaveChangesAsync();
    }

    [Test]
    public async Task Should_create_building_when_request_is_valid()
    {
        var floors = TestDataGenerator.GetTestFloors([1,2,3,4]);
        var testBuilding = new CreateBuildingRequest
        {
            Title = "TestBuilding",
            Description = "TestDescription",
            Floors = floors
        };

        var createBuildingResponse = await client.CreateBuildingAsync(testBuilding);
        createBuildingResponse.IsSuccess.Should().BeTrue();
        var buildingId = createBuildingResponse.Data.BuildingId;
        
        var getBuildingOperation = await client.GetBuildingByIdAsync(buildingId);
        getBuildingOperation.IsSuccess.Should().BeTrue();
        var building = getBuildingOperation.Data;
        
        building.Id.Should().Be(buildingId);
        building.Title.Should().Be(testBuilding.Title);
        building.Floors.Should().BeEquivalentTo(floors);
        building.Description.Should().Be(testBuilding.Description);
    }
    
    [Test]
    public async Task Should_delete_building_when_request_is_valid()
    {
        var buildingIds = await TestDataGenerator.CreateRandomBuildings(client, 1).ConfigureAwait(false);
        var buildingId = buildingIds.First();
        
        var getBuildingOperation = await client.GetBuildingByIdAsync(buildingId);
        getBuildingOperation.IsSuccess.Should().BeTrue();
        
        var deleteBuildingOperation = await client.DeleteBuildingAsync(buildingId).ConfigureAwait(false);
        deleteBuildingOperation.IsSuccess.Should().BeTrue();
        
        getBuildingOperation = await client.GetBuildingByIdAsync(buildingId);
        getBuildingOperation.IsSuccess.Should().BeFalse();
        getBuildingOperation.ApiError.Urn.Should().BeEquivalentTo(BuildingsApiErrors.BuildingWithIdNotFoundError(buildingId).Urn);
    }
    
    [Test]
    public async Task Should_return_one_building_when_filter_by_id()
    {
        var buildingIds = await TestDataGenerator.CreateRandomBuildings(client, 10).ConfigureAwait(false);

        var getTestBuilding = await client.GetBuildingByIdAsync(buildingIds.First()).ConfigureAwait(false);
        getTestBuilding.IsSuccess.Should().BeTrue();
        
        var testBuilding = getTestBuilding.Data;
        var filter = new BuildingFilter
        {
            BuildingId = testBuilding.Id
        };
        
        var getAllBuildingsOperation = await client.GetAllByFilterAsync(filter).ConfigureAwait(false);
        getAllBuildingsOperation.IsSuccess.Should().BeTrue();
        getAllBuildingsOperation.Data.Should().HaveCount(1);
        var building = getAllBuildingsOperation.Data.First();
        
        building.Id.Should().Be(testBuilding.Id);
        building.Title.Should().Be(testBuilding.Title);
        building.Floors.Should().BeEquivalentTo(testBuilding.Floors);
        building.Description.Should().Be(testBuilding.Description);
    }
    
    [Test]
    public async Task Should_return_one_building_when_filter_by_title()
    {
        var buildingIds = await TestDataGenerator.CreateRandomBuildings(client,10).ConfigureAwait(false);

        var getTestBuilding = await client.GetBuildingByIdAsync(buildingIds.First()).ConfigureAwait(false);
        getTestBuilding.IsSuccess.Should().BeTrue();
        
        var testBuilding = getTestBuilding.Data;
        var filter = new BuildingFilter
        {
            Title = testBuilding.Title
        };
        
        var getAllBuildingsOperation = await client.GetAllByFilterAsync(filter).ConfigureAwait(false);
        getAllBuildingsOperation.IsSuccess.Should().BeTrue();
        getAllBuildingsOperation.Data.Should().HaveCount(1);
        
        var building = getAllBuildingsOperation.Data.First();
        
        building.Id.Should().Be(testBuilding.Id);
        building.Title.Should().Be(testBuilding.Title);
        building.Floors.Should().BeEquivalentTo(testBuilding.Floors);
        building.Description.Should().Be(testBuilding.Description);
    }
    
    [Test]
    public async Task Should_return_correct_count_of_buildings_when_take()
    {
        await TestDataGenerator.CreateRandomBuildings(client, 10).ConfigureAwait(false);

        var filter = new BuildingFilter() { Take = 5 };
        
        var getAllBuildingsOperation = await client.GetAllByFilterAsync(filter).ConfigureAwait(false);
        getAllBuildingsOperation.IsSuccess.Should().BeTrue();
        getAllBuildingsOperation.Data.Should().HaveCount(5);
    }
    
    [Test]
    public async Task Should_return_correct_count_of_buildings_when_skip()
    {
        await TestDataGenerator.CreateRandomBuildings(client,10).ConfigureAwait(false);

        var filter = new BuildingFilter() { Skip = 5 };
        
        var getAllBuildingsOperation = await client.GetAllByFilterAsync(filter).ConfigureAwait(false);
        getAllBuildingsOperation.IsSuccess.Should().BeTrue();
        getAllBuildingsOperation.Data.Should().HaveCount(5);
    }
    
    [Test]
    public async Task Should_return_correct_count_of_buildings_when_take_and_skip()
    {
        await TestDataGenerator.CreateRandomBuildings(client,10).ConfigureAwait(false);

        var filter = new BuildingFilter() { Skip = 5, Take = 2};
        
        var getAllBuildingsOperation = await client.GetAllByFilterAsync(filter).ConfigureAwait(false);
        getAllBuildingsOperation.IsSuccess.Should().BeTrue();
        getAllBuildingsOperation.Data.Should().HaveCount(2);
    }

    [TestCaseSource(nameof(TestCasesWithWrongFloorNumbers))]
    public async Task Should_return_bad_request_when_floor_numbers_is_invalid(int[] floorNumbers)
    {
        var floors = TestDataGenerator.GetTestFloors(floorNumbers);
        var testBuilding = new CreateBuildingRequest
        {
            Title = "TestBuilding",
            Description = "TestDescription", 
            Floors = floors
        };
        
        var operationResult = await client.CreateBuildingAsync(testBuilding);
        
        operationResult.IsSuccess.Should().BeFalse();
        
        var error = operationResult.ApiError;
        var expectedError = BuildingsApiErrors.WrongFloorNumberError(1, 1);
        error.Urn.Should().BeEquivalentTo(expectedError.Urn);
    }

    [Test]
    public async Task GetBuildingById_Should_return_404_when_building_does_not_exist()
    {
        var buildingId = Guid.NewGuid();
        var getBuildingOperation = await client.GetBuildingByIdAsync(buildingId).ConfigureAwait(false);
        
        getBuildingOperation.IsSuccess.Should().BeFalse();
        getBuildingOperation.ApiError.Urn.Should().BeEquivalentTo(BuildingsApiErrors.BuildingWithIdNotFoundError(buildingId).Urn);
    }

    public static IEnumerable<int[]> TestCasesWithWrongFloorNumbers
    {
        get
        {
            yield return new[] {-1 };
            yield return new[] { 0 };
            yield return new[] { 2 };
            yield return new[] { 1, 3};
            yield return new[] { 1, -1 };
        }
    }

    
}