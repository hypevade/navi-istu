using System.Reflection;
using Istu.Navigation.Api.Middlewares;
using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Public.Models;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<BuildingsDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("BuildingsDatabase")));
        
        services.AddSwaggerGen();
        
        services.AddAutoMapper(typeof(PublicMappingProfile).Assembly);
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddScoped<DbContext, BuildingsDbContext>();
        services.AddScoped<IBuildingObjectsRepository, BuildingObjectsRepository>();
        services.AddScoped<IBuildingsRepository, BuildingsRepository>();
        services.AddScoped<IEdgesRepository, EdgesRepository>();
        services.AddScoped<IFloorsRepository, FloorsRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IBuildingRoutesService, BuildingRoutesService>();
        services.AddScoped<IBuildingObjectsService, BuildingObjectsService>();
        services.AddScoped<IBuildingsService, BuildingsService>();
        services.AddScoped<IFloorsService, FloorsService>();
        services.AddScoped<IEdgesService, EdgesService>();
        services.AddScoped<IImageService, ImageService>();
        
        services.AddSingleton<IRouteSearcher, RouteSearcher>();
        
        services.AddControllers();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerUI();
        }

        using var scope = app.ApplicationServices.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<BuildingsDbContext>();
        dbContext.Database.EnsureCreated();
        
        

        app.UseRouting();

        app.UseAuthorization();
        
        app.UseMiddleware<ErrorHandlingMiddleware>();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}