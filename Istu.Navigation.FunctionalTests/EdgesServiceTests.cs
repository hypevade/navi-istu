using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.TestClient;
using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.FunctionalTests;

public class EdgesServiceTests
{
    private IEdgesClient client = null!;
    private BuildingsDbContext dbContext = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        var istuNavigationTestClient = await WebHostInstaller.GetHttpClient().ConfigureAwait(false);
        client = istuNavigationTestClient.Client.EdgesClient;
        dbContext = istuNavigationTestClient.DbContext;
    }

    [Test]
    public async Task METHOD()
    {
        var building = new BuildingEntity()
        {
            Id = Guid.NewGuid(),
            Title = "TestBuilding",
            FloorNumbers = 5,
            Description = "TestDescription",
            IsDeleted = false
        };
        var buildingObject = new BuildingObjectEntity()
        {
            Id = Guid.NewGuid(),
            Title = "TestBuildingObject",
            Floor = 5,
            Description = "TestDescription",
            IsDeleted = false,
            Type = BuildingObjectType.Node,
            BuildingId = building.Id,
            X = 0,
            Y = 0
        };
        
        var buildingObject2 = new BuildingObjectEntity()
        {
            Id = Guid.NewGuid(),
            Title = "TestBuildingObject2",
            Floor = 5,
            Description = "TestDescription2",
            IsDeleted = false,
            Type = BuildingObjectType.Node,
            BuildingId = building.Id,
            X = 1,
            Y = 1
        };
        var edge = new EdgeEntity()
        {
            Id = Guid.NewGuid(),
            BuildingId = building.Id,
            FromObject = buildingObject.Id,
            ToObject = buildingObject2.Id,
            FloorNumber = 5,
            IsDeleted = false
        };
        await dbContext.Buildings.AddAsync(building).ConfigureAwait(false);
        await dbContext.Objects.AddAsync(buildingObject).ConfigureAwait(false);
        await dbContext.Objects.AddAsync(buildingObject2).ConfigureAwait(false);
        await dbContext.Edges.AddAsync(edge).ConfigureAwait(false);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
        
        var filter = new EdgeFilter()
        {
            BuildingObjectId = buildingObject.Id,
            Skip = 0,
            Take = 11
        };
        var res = await client.GetAllEdgesByFilter(filter).ConfigureAwait(false);
    }
    
}