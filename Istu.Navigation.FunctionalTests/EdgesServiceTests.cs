using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.TestClient;
using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.FunctionalTests;

public class EdgesServiceTests
{
    private IEdgesClient client = null!;
    private BuildingsDbContext dbContext = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var istuNavigationTestClient = await WebHostInstaller.GetHttpClient().ConfigureAwait(false);
        client = istuNavigationTestClient.Client.EdgesClient;
        dbContext = istuNavigationTestClient.DbContext;
    }

    [Test]
    public async Task METHOD()
    {
        
    }
    
    [TearDown]
    public async Task TearDown()
    {
        dbContext.Buildings.RemoveRange(dbContext.Buildings);
        dbContext.ImageLinks.RemoveRange(dbContext.ImageLinks);
        await dbContext.SaveChangesAsync();
    }
}