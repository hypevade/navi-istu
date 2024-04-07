using Istu.Navigation.Api;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.TestClient;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Istu.Navigation.FunctionalTests;

public class WebHostInstaller
{
    public static async Task<IstuNavigationTestClient> GetHttpClient()
    {
        var webHost = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContext =
                    services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<BuildingsDbContext>));
                if (dbContext != null)
                    services.Remove(dbContext);
                services.AddDbContext<BuildingsDbContext>(options =>
                {
                    options.UseInMemoryDatabase("BuildingsDatabase");
                });
            });
        });

        /*var dbContext = webHost.Services.CreateScope().ServiceProvider.GetService<BuildingsDbContext>();
        
        var building1 = new BuildingEntity()
        {
            Id = Guid.Parse("2f1b7f5e-4f8b-4f8b-8f8b-8f8b8f8b8f8b"),
            Title = "Building1",
            Description = "Building1_Description",
            IsDeleted = false,
            FloorNumbers = 1,
        };
        var building2 = new BuildingEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Building2",
            Description = "Building2_Description",
            IsDeleted = false,
            FloorNumbers = 1,
        };
        var buildings = new List<BuildingEntity> {building1, building2};
        await dbContext.Buildings.AddRangeAsync(buildings).ConfigureAwait(false);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);*/
        return IstuNavigationTestClient.Create(webHost.CreateClient());
    }
}