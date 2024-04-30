using FluentAssertions;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.Buildings;
using Istu.Navigation.Public.Models.Edges;
using Istu.Navigation.TestClient;
using Istu.Navigation.TestClient.SubsidiaryClients;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.FunctionalTests;

public class EdgesServiceTests
{
    private IEdgesClient client = null!;
    private IstuNavigationTestClient commonClient = null!;
    private BuildingsDbContext dbContext = null!;
    private BuildingDto testBuilding = null!;
    private BuildingObjectDto testBuildingObject1 = null!;
    private BuildingObjectDto testBuildingObject2 = null!;
    

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var istuNavigationTestClient = await WebHostInstaller.GetHttpClient().ConfigureAwait(false);
        commonClient = istuNavigationTestClient.Client;
        client = istuNavigationTestClient.Client.EdgesClient;
        dbContext = istuNavigationTestClient.DbContext;
        var createOperation =
            await TestDataGenerator.CreateRandomBuildings(istuNavigationTestClient.Client.BuildingsClient, 1)
                .ConfigureAwait(false);
        
        var getBuildingOperation = await istuNavigationTestClient.Client.BuildingsClient
            .GetBuildingByIdAsync(createOperation.First())
            .ConfigureAwait(false);

        testBuilding = getBuildingOperation.Data;
        var objects = TestDataGenerator.GenerateBuildingObjects(2, testBuilding.Id);
        var create1 = await commonClient.BuildingObjectsClient.CreateBuildingObjectAsync(objects[0]).ConfigureAwait(false);
        var create2 = await commonClient.BuildingObjectsClient.CreateBuildingObjectAsync(objects[1]).ConfigureAwait(false);
        create1.IsSuccess.Should().BeTrue();
        create2.IsSuccess.Should().BeTrue();
        var boId1 = create1.Data.BuildingObjectId;
        var boId2 = create2.Data.BuildingObjectId;
        var getObj = await commonClient.BuildingObjectsClient.GetBuildingObjectByIdAsync(boId1).ConfigureAwait(false);
        var getObj2 = await commonClient.BuildingObjectsClient.GetBuildingObjectByIdAsync(boId2).ConfigureAwait(false);
        testBuildingObject1 = getObj.Data;
        testBuildingObject2 = getObj2.Data;
    }

    [Test]
    public async Task CreateEdge_Should_create_edge_with_correct_request()
    {
        var request = new CreateEdgesRequest()
        {
            Edges = [new(testBuildingObject1.Id, testBuildingObject2.Id)]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeTrue();
        create.Data.EdgeIds.Should().HaveCount(1);
        await ShouldCreateEdge(testBuildingObject1, testBuildingObject2, create.Data.EdgeIds.First())
            .ConfigureAwait(false);
    }
    
    [Test]
    public async Task CreateEdge_Should_return_same_edge_id_when_exist()
    {
        var request = new CreateEdgesRequest()
        {
            Edges = [new(testBuildingObject1.Id, testBuildingObject2.Id)]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        var create2 = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeTrue();
        create2.IsSuccess.Should().BeTrue();
        create.Data.EdgeIds.Should().HaveCount(1);
        create2.Data.EdgeIds.Should().HaveCount(1);
        create.Data.EdgeIds.First().Should().Be(create2.Data.EdgeIds.First());
        await ShouldCreateEdge(testBuildingObject1, testBuildingObject2, create.Data.EdgeIds.First())
            .ConfigureAwait(false);
    }
    
    [Test]
    public async Task CreateEdge_Should_return_404_when_building_object_not_found()
    {
        var fakeId = Guid.NewGuid();
        var request = new CreateEdgesRequest()
        {
            Edges = [new(testBuildingObject1.Id, fakeId)]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeFalse();
        create.ApiError.Should().BeEquivalentTo(BuildingObjectsApiErrors.BuildingObjectNotFoundError(fakeId));
        create.ApiError.StatusCode.Should().Be(404);
    }
    [Test]
    public async Task CreateEdge_Should_return_404_when_no_objects()
    {
        var fakeId = Guid.NewGuid();
        var fakeId2 = Guid.NewGuid();
        var request = new CreateEdgesRequest()
        {
            Edges = [new(fakeId, fakeId2)]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeFalse();
        create.ApiError.Urn.Should().BeEquivalentTo(BuildingObjectsApiErrors.BuildingObjectNotFoundError(fakeId).Urn);
        create.ApiError.StatusCode.Should().Be(404);
    }
    
    [Test]
    public async Task CreateEdge_Should_return_400_when_objects_from_different_buildings()
    {
        var createBuilding = await TestDataGenerator.CreateRandomBuildings(commonClient.BuildingsClient,1).ConfigureAwait(false);
        var testBuildingId = createBuilding.First();
        var objects = TestDataGenerator.GenerateBuildingObjects(1, testBuildingId);
        var createObject = await commonClient.BuildingObjectsClient.CreateBuildingObjectAsync(objects[0]).ConfigureAwait(false);
        createObject.IsSuccess.Should().BeTrue();
        var request = new CreateEdgesRequest()
        {
            Edges = [new(testBuildingObject1.Id, createObject.Data.BuildingObjectId)]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeFalse();
        create.ApiError.Should().BeEquivalentTo(EdgesApiErrors.EdgeFromDifferentBuildingsError(testBuildingObject1.Id, createObject.Data.BuildingObjectId));
        create.ApiError.StatusCode.Should().Be(400);
    }

    [Test]
    public async Task GetAllByFilter_Should_return_all_edges_when_from_id()
    {
        var count = 4;
        var boIds = await TestDataGenerator
            .CreateRandomBuildingsObjects(commonClient.BuildingObjectsClient, testBuilding.Id, count)
            .ConfigureAwait(false);
        boIds.Should().HaveCount(count);
        var request = new CreateEdgesRequest()
        {
            Edges =
            [
                new(boIds[0], boIds[1]),
                new(boIds[0], boIds[2]),
                new(boIds[0], boIds[3]),
                new(boIds[1], boIds[3]),
            ]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeTrue();
        create.Data.EdgeIds.Should().HaveCount(request.Edges.Count);
        
        var filter = new EdgeFilter()
        {
            FromBuildingObjectId = boIds[0]
        };
        var edges = await client.GetAllEdgesByFilter(filter).ConfigureAwait(false);
        edges.IsSuccess.Should().BeTrue();
        edges.Data.Should().HaveCount(3);
        edges.Data.ForEach(x => x.FromId.Should().Be(boIds[0]));
    }
    
    [Test]
    public async Task GetAllByFilter_Should_return_all_edges_when_to_id()
    {
        var count = 4;
        var boIds = await TestDataGenerator
            .CreateRandomBuildingsObjects(commonClient.BuildingObjectsClient, testBuilding.Id, count)
            .ConfigureAwait(false);
        boIds.Should().HaveCount(count);
        var request = new CreateEdgesRequest()
        {
            Edges =
            [
                new(boIds[0], boIds[1]),
                new(boIds[0], boIds[2]),
                new(boIds[0], boIds[3]),
                new(boIds[1], boIds[3]),
            ]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeTrue();
        create.Data.EdgeIds.Should().HaveCount(request.Edges.Count);
        var filter = new EdgeFilter()
        {
            ToBuildingObjectId = boIds[0]
        };
        var edges = await client.GetAllEdgesByFilter(filter).ConfigureAwait(false);
        edges.IsSuccess.Should().BeTrue();
        edges.Data.Should().HaveCount(3);
        edges.Data.ForEach(x => x.FromId.Should().Be(boIds[0]));
    }
    
    [Test]
    public async Task GetAllByFilter_Should_return_one_edge_when_from_id_and_to_id()
    {
        var count = 3;
        var boIds = await TestDataGenerator
            .CreateRandomBuildingsObjects(commonClient.BuildingObjectsClient, testBuilding.Id, count)
            .ConfigureAwait(false);
        boIds.Should().HaveCount(count);
        var request = new CreateEdgesRequest()
        {
            Edges =
            [
                new(boIds[0], boIds[1]),
                new(boIds[0], boIds[2]),
            ]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeTrue();
        create.Data.EdgeIds.Should().HaveCount(request.Edges.Count);
        var filter = new EdgeFilter()
        {
            ToBuildingObjectId = boIds[0],
            FromBuildingObjectId = boIds[1]
        };
        var edges = await client.GetAllEdgesByFilter(filter).ConfigureAwait(false);
        edges.IsSuccess.Should().BeTrue();
        edges.Data.Should().HaveCount(1);
        edges.Data.First().Should().BeEquivalentTo(request.Edges.First());
    }

    [Test]
    public async Task DeleteEdge_Should_successful_delete_when_exist()
    {
        var request = new CreateEdgesRequest()
        {
            Edges = [new(testBuildingObject1.Id, testBuildingObject2.Id)]
        };
        var create = await client.CreateEdges(request).ConfigureAwait(false);
        create.IsSuccess.Should().BeTrue();
        create.Data.EdgeIds.Should().HaveCount(1);
        await ShouldCreateEdge(testBuildingObject1, testBuildingObject2, create.Data.EdgeIds.First())
            .ConfigureAwait(false);
        var delete = await client.DeleteEdge(create.Data.EdgeIds.First()).ConfigureAwait(false);
        delete.IsSuccess.Should().BeTrue();
        dbContext.Edges.FirstOrDefault(x => x.Id == create.Data.EdgeIds.First()).Should().BeNull();
    }
    
    [Test]
    public async Task DeleteEdge_Should_return_success_when_not_exist()
    {
        var delete = await client.DeleteEdge(Guid.NewGuid()).ConfigureAwait(false);
        delete.IsSuccess.Should().BeTrue();
    }
    
    private async Task ShouldCreateEdge(BuildingObjectDto from, BuildingObjectDto to, Guid edgeId)
    {
        var edgeEntity = await dbContext.Edges.FirstOrDefaultAsync(x => x.Id == edgeId)
            .ConfigureAwait(false);
        edgeEntity.Should().NotBeNull();
        if(edgeEntity is null) 
            return;
        
        edgeEntity.BuildingId.Should().Be(from.BuildingId);
        edgeEntity.FloorNumber.Should().Be(from.Floor);
        edgeEntity.FromObject.Should().Be(from.Id);
        edgeEntity.ToObject.Should().Be(to.Id);
    }

    [TearDown]
    public async Task TearDown()
    {
        dbContext.Edges.RemoveRange(dbContext.Edges);
        dbContext.ImageLinks.RemoveRange(dbContext.ImageLinks);
        await dbContext.SaveChangesAsync();
    }
}