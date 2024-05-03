using FluentAssertions;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.Buildings;
using Istu.Navigation.TestClient;
using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.FunctionalTests;

public class BuildingObjectsServiceTests
{
    private IBuildingObjectsClient client = null!;
    private AppDbContext dbContext = null!;
    private BuildingDto testBuilding = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var istuNavigationTestClient = await WebHostInstaller.GetHttpClient().ConfigureAwait(false);
        client = istuNavigationTestClient.Client.BuildingObjectsClient;
        dbContext = istuNavigationTestClient.DbContext;
        var createOperation =
            await TestDataGenerator.CreateRandomBuildings(istuNavigationTestClient.Client.BuildingsClient, 1)
                .ConfigureAwait(false);

        var getBuildingOperation = await istuNavigationTestClient.Client.BuildingsClient
            .GetBuildingByIdAsync(createOperation.First())
            .ConfigureAwait(false);

        testBuilding = getBuildingOperation.Data;
    }

    [TearDown]
    public async Task TearDown()
    {
        dbContext.Objects.RemoveRange(dbContext.Objects);
        await dbContext.SaveChangesAsync();
    }

    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    [TestCase(2, 0)]
    [TestCase(0, 2)]
    public async Task Should_return_bad_request_when_invalid_coordinates(int x, int y)
    {
        var points = new List<(double X, double Y)>
        {
            (x, y)
        };
        var objects = TestDataGenerator.GenerateBuildingObjects(points, testBuilding.Id);

        var createResponse = objects.First();
        var createOperation = await client.CreateBuildingObjectAsync(createResponse).ConfigureAwait(false);
        createOperation.IsFailure.Should().BeTrue();
        var error = createOperation.ApiError;
        var expectedError = BuildingObjectsApiErrors.InvalidCoordinatesError(x, y);
        error.Should().BeEquivalentTo(expectedError);
    }

    [Test]
    public async Task Should_create_building_object_when_request_is_valid()
    {
        var requests = TestDataGenerator.GenerateBuildingObjects(1, testBuilding.Id);
        var createResponse = requests.First();
        var createOperation = await client.CreateBuildingObjectAsync(createResponse).ConfigureAwait(false);
        createOperation.IsSuccess.Should().BeTrue();

        var buildingObjectId = createOperation.Data.BuildingObjectId;
        var getOperation = await client.GetBuildingObjectByIdAsync(buildingObjectId).ConfigureAwait(false);
        getOperation.IsSuccess.Should().BeTrue();
        var buildingObject = getOperation.Data;
        buildingObject.Id.Should().Be(buildingObjectId);

        ResponseShouldBeEqual(createResponse, buildingObject);
    }

    [Test]
    public async Task Should_return_building_object_by_filter_with_object_id()
    {
        var requests = TestDataGenerator.GenerateBuildingObjects(10, testBuilding.Id);
        var ids = await CreateBuildingObjects(requests).ConfigureAwait(false);

        ids.Should().NotBeEmpty();
        requests.Should().NotBeEmpty();
        var buildingObjectRequest = requests.First();
        var testObjId = ids.First();
        var filterById = new BuildingObjectFilter()
        {
            BuildingId = testObjId
        };
        var getOperation = await client.GetAllByFilterAsync(filterById).ConfigureAwait(false);
        getOperation.IsSuccess.Should().BeTrue();
        var buildingObject = getOperation.Data.First();
        buildingObject.Id.Should().Be(testObjId);

        ResponseShouldBeEqual(buildingObjectRequest, buildingObject);
    }

    [Test]
    public async Task Should_return_building_object_by_filter_with_title()
    {
        var requests = TestDataGenerator.GenerateBuildingObjects(10, testBuilding.Id);
        var ids = await CreateBuildingObjects(requests).ConfigureAwait(false);

        ids.Should().NotBeEmpty();
        requests.Should().NotBeEmpty();
        var buildingObjectRequest = requests.First();
        var testObjId = ids.First();

        var filterById = new BuildingObjectFilter
        {
            Title = buildingObjectRequest.Title
        };
        var getOperation = await client.GetAllByFilterAsync(filterById).ConfigureAwait(false);
        getOperation.IsSuccess.Should().BeTrue();
        var buildingObject = getOperation.Data.First();
        buildingObject.Id.Should().Be(testObjId);

        ResponseShouldBeEqual(buildingObjectRequest, buildingObject);
    }

    [Test]
    public async Task Should_return_building_objects_by_filter_with_floor()
    {
        var numberOfBuildingObjects = 10;
        var requests = TestDataGenerator.GenerateBuildingObjects(numberOfBuildingObjects, testBuilding.Id);
        var ids = await CreateBuildingObjects(requests).ConfigureAwait(false);

        var filterById = new BuildingObjectFilter
        {
            Floor = 1,
            BuildingId = testBuilding.Id
        };
        var getOperation = await client.GetAllByFilterAsync(filterById).ConfigureAwait(false);
        getOperation.IsSuccess.Should().BeTrue();
        var objects = getOperation.Data;
        objects.Should().HaveCount(numberOfBuildingObjects);
        objects.Select(x => x.Id).Should().BeEquivalentTo(ids);
    }

    private void ResponseShouldBeEqual(CreateBuildingObjectRequest request, BuildingObjectDto response)
    {
        response.Title.Should().Be(request.Title);
        response.BuildingId.Should().Be(request.BuildingId);
        response.Floor.Should().Be(request.Floor);
        response.Type.Should().Be(request.Type);
        response.X.Should().Be(request.X);
        response.Y.Should().Be(request.Y);
    }

    private async Task<List<Guid>> CreateBuildingObjects(List<CreateBuildingObjectRequest> requests)
    {
        var ids = new List<Guid>();
        foreach (var request in requests)
        {
            var createOperation = await client.CreateBuildingObjectAsync(request).ConfigureAwait(false);
            ids.Add(createOperation.Data.BuildingObjectId);
        }

        return ids;
    }
}
