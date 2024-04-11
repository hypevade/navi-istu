using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Istu.Navigation.Infrastructure.EF;

public class DbContextFactory : IDesignTimeDbContextFactory<BuildingsDbContext>
{
    public BuildingsDbContext CreateDbContext(string[] args)
    {
        var projectApiDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../Istu.Navigation.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectApiDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var dbContextBuilder = new DbContextOptionsBuilder<BuildingsDbContext>();

        var connectionString = configuration.GetConnectionString("BuildingsDatabaseTest");

        dbContextBuilder.UseNpgsql(connectionString);

        return new BuildingsDbContext(dbContextBuilder.Options);
    }
}