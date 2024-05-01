using Istu.Navigation.Api;
using Istu.Navigation.Infrastructure.EF;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Istu.Navigation.TestClient;

public class WebHostInstaller
{
    public static async Task<(IstuNavigationTestClient Client, AppDbContext DbContext)> GetHttpClient()
    {
        var webHost = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContext =
                    services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbContext != null)
                    services.Remove(dbContext);
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("BuildingsDatabase");
                });
            });
        });
        var client = IstuNavigationTestClient.Create(webHost.CreateClient());
        var dbContext = webHost.Services.CreateScope().ServiceProvider.GetService<AppDbContext>();
        return (client, dbContext!);
    }
}