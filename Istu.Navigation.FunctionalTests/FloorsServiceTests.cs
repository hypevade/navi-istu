using FluentAssertions;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using Istu.Navigation.Public.Models.Buildings;
using Istu.Navigation.TestClient;
using Istu.Navigation.TestClient.SubsidiaryClients;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.FunctionalTests;

public class FloorsServiceTests
{
    private IBuildingsClient client = null!;
    private AppDbContext dbContext = null!;
    private BuildingDto buildingDto = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var istuNavigationTestClient = await WebHostInstaller.GetHttpClient().ConfigureAwait(false);
        client = istuNavigationTestClient.Client.BuildingsClient;
        dbContext = istuNavigationTestClient.DbContext;

        var buildings = await TestDataGenerator.CreateRandomBuildings(client, 1).ConfigureAwait(false);
        var getBuilding = await client.GetBuildingByIdAsync(buildings.First()).ConfigureAwait(false);
        buildingDto = getBuilding.Data;
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(5)]
    public async Task CreateFloor_Should_create_floor_when_request_is_valid(int floorNumber)
    {
        var createFloorRequest = TestDataGenerator.GetCreateFloorRequest(floorNumber: floorNumber);
        var floor = await client.AddFloorAsync(buildingDto.Id, createFloorRequest).ConfigureAwait(false);
        floor.IsSuccess.Should().BeTrue();

        await ShouldCreateFloor(buildingDto.Id, floor.Data.FloorId, createFloorRequest.ImageLink, floorNumber);
    }

    [Test]
    public async Task GetFloor_Should_return_floor_when_exist()
    {
        var floorNumber = 1;
        var createFloorRequest = TestDataGenerator.GetCreateFloorRequest(floorNumber: floorNumber);
        var floor = await client.AddFloorAsync(buildingDto.Id, createFloorRequest).ConfigureAwait(false);
        floor.IsSuccess.Should().BeTrue();

        await ShouldCreateFloor(buildingDto.Id, floor.Data.FloorId, createFloorRequest.ImageLink, floorNumber);
        var get = await client.GetFloorAsync(buildingDto.Id, floorNumber).ConfigureAwait(false);
        get.IsSuccess.Should().BeTrue();
        var floorDto = get.Data;
        floorDto.FloorNumber.Should().Be(floorNumber);
        floorDto.ImageLink.Should().Be(createFloorRequest.ImageLink);
        floorDto.Edges.Should().BeEmpty();
        floorDto.Objects.Should().BeEmpty();
    }

    [Test]
    public async Task GetFloor_Should_return_404_when_floor_not_exist()
    {
        var floorNumber = 1;
        var createFloorRequest = TestDataGenerator.GetCreateFloorRequest(floorNumber: floorNumber);
        var floor = await client.AddFloorAsync(buildingDto.Id, createFloorRequest).ConfigureAwait(false);
        floor.IsSuccess.Should().BeTrue();

        await ShouldCreateFloor(buildingDto.Id, floor.Data.FloorId, createFloorRequest.ImageLink, floorNumber);
        var get = await client.GetFloorAsync(buildingDto.Id, 2).ConfigureAwait(false);
        get.IsSuccess.Should().BeFalse();
        var error = get.ApiError;
        error.StatusCode.Should().Be(404);
        error.Should()
            .BeEquivalentTo(BuildingsApiErrors.FloorWithBuildingAndFloorNumberNotFoundError(buildingDto.Id, 2));
    }

    [Test]
    public async Task DeleteFloor_Should_return_accepted_when_floor_exist()
    {
        var floorNumber = 1;
        var createFloorRequest = TestDataGenerator.GetCreateFloorRequest(floorNumber: floorNumber);
        var floor = await client.AddFloorAsync(buildingDto.Id, createFloorRequest).ConfigureAwait(false);
        floor.IsSuccess.Should().BeTrue();

        await ShouldCreateFloor(buildingDto.Id, floor.Data.FloorId, createFloorRequest.ImageLink, floorNumber);
        var delete = await client.DeleteFloorAsync(buildingDto.Id, floorNumber).ConfigureAwait(false);
        delete.IsSuccess.Should().BeTrue();

        var get = dbContext.Floors.FirstOrDefault(x => x.BuildingId == buildingDto.Id && x.FloorNumber == floorNumber);
        get.Should().BeNull();
    }

    [Test]
    public async Task DeleteFloor_Should_return_accepted_when_floor_not_exist()
    {
        var floorNumber = 1;
        var delete = await client.DeleteFloorAsync(buildingDto.Id, floorNumber).ConfigureAwait(false);
        delete.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task CreateFloor_Should_create_floor_when_request_without_floor_number()
    {
        var createFloorRequest = TestDataGenerator.GetCreateFloorRequest();

        var floor = await client.AddFloorAsync(buildingDto.Id, createFloorRequest).ConfigureAwait(false);
        floor.IsSuccess.Should().BeTrue();
        await ShouldCreateFloor(buildingDto.Id, floor.Data.FloorId, createFloorRequest.ImageLink, 1);
    }
    
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public async Task CreateFloor_Should_return_400_when_floor_number_less_than_one(int floorNumber)
    {
        var createFloorRequest = TestDataGenerator.GetCreateFloorRequest(floorNumber: floorNumber);

        var floor = await client.AddFloorAsync(buildingDto.Id, createFloorRequest).ConfigureAwait(false);
        floor.IsSuccess.Should().BeFalse();
        floor.ApiError.StatusCode.Should().Be(400);
        floor.ApiError.Should().BeEquivalentTo(BuildingsApiErrors.FloorNumberLessThanMinFloorError(floorNumber));
    }

    [Test]
    public async Task CreateFloor_Should_create_floors_two_requests_without_floor_number()
    {
        var createFloorRequest1 = TestDataGenerator.GetCreateFloorRequest();
        var createFloorRequest2 = TestDataGenerator.GetCreateFloorRequest();

        var floor1 = await client.AddFloorAsync(buildingDto.Id, createFloorRequest1).ConfigureAwait(false);
        var floor2 = await client.AddFloorAsync(buildingDto.Id, createFloorRequest2).ConfigureAwait(false);

        floor1.IsSuccess.Should().BeTrue();
        await ShouldCreateFloor(buildingDto.Id, floor1.Data.FloorId, createFloorRequest1.ImageLink, 1);
        await ShouldCreateFloor(buildingDto.Id, floor2.Data.FloorId, createFloorRequest2.ImageLink, 2);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(" \n ")]
    public async Task CreateFloor_Should_return_error_when_link_is_invalid(string invalidLink)
    {
        var createFloorRequest = TestDataGenerator.GetCreateFloorRequest(link: invalidLink);
        var floor = await client.AddFloorAsync(buildingDto.Id, createFloorRequest).ConfigureAwait(false);
        floor.IsSuccess.Should().BeFalse();
        floor.ApiError.Urn.Should().Be(ImagesApiErrors.ImageWithEmptyLinkError().Urn);
    }

    [Test]
    public async Task CreateFloor_Should_return_error_when_building_not_found()
    {
        var fakeBuildingId = Guid.NewGuid();
        var createFloorRequest = TestDataGenerator.GetCreateFloorRequest();
        var floor = await client.AddFloorAsync(fakeBuildingId, createFloorRequest).ConfigureAwait(false);
        floor.IsSuccess.Should().BeFalse();
        floor.ApiError.Should().BeEquivalentTo(BuildingsApiErrors.BuildingWithIdNotFoundError(fakeBuildingId));
    }

    private async Task ShouldCreateFloor(Guid buildingId, Guid floorId, string imageLink, int floorNumber)
    {
        var floorEntity = await dbContext.Floors.FirstOrDefaultAsync(x => x.Id == floorId);
        floorEntity.Should().NotBeNull();

        floorEntity?.BuildingId.Should().Be(buildingId);
        floorEntity?.FloorNumber.Should().Be(floorNumber);

        var image = await dbContext.ImageLinks.FirstOrDefaultAsync(x => x.ObjectId == floorId)
            .ConfigureAwait(false);
        image.Should().NotBeNull();
        image?.Link.Should().Be(imageLink);
    }

    [TearDown]
    public async Task TearDown()
    {
        dbContext.Floors.RemoveRange(dbContext.Floors);
        dbContext.ImageLinks.RemoveRange(dbContext.ImageLinks);
        await dbContext.SaveChangesAsync();
    }
}