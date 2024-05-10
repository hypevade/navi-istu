using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Istu.Navigation.Infrastructure.EF;

public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var projectApiDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../Istu.Navigation.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectApiDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var dbContextBuilder = new DbContextOptionsBuilder<AppDbContext>();

        //var connectionString = configuration.GetConnectionString("BuildingsDatabaseTest");
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__TestDataBase");

        dbContextBuilder.UseNpgsql(connectionString);

        return new AppDbContext(dbContextBuilder.Options);
    }
}