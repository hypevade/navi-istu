using System.Text;
using Istu.Navigation.Api.Middlewares;
using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.ExternalRoutes;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Domain.Repositories.Users;
using Istu.Navigation.Domain.Services.BuildingRoutes;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Domain.Services.ExternalRoutes;
using Istu.Navigation.Domain.Services.Users;
using Istu.Navigation.Infrastructure.Common;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Public.Models;
using Itinero;
using Itinero.IO.Osm;
using Itinero.Osm.Vehicles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Istu.Navigation.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }
    public void ConfigureExternalSearcher(IServiceCollection services)
    {
        var routerDb = new RouterDb();
        var pathToMap = Configuration.GetSection("Map").GetSection("Path").Value ?? string.Empty;
        using (var stream = new FileInfo(pathToMap).OpenRead())
        {
            //TODO: Если будем добавлять не только пешеходные маршруты, следует добавить Vehicle.Car и Vehicle.Bike
            routerDb.LoadOsmData(stream, Vehicle.Pedestrian);
        }
        services.Configure<MapOptions>(Configuration.GetSection("Map"));

        services.AddSingleton(routerDb);
        services.AddScoped<Router>();
        services.AddScoped<IExternalRoutesSearcher, ExternalRoutesSearcher>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__TestDataBase");
        var privateKey = Environment.GetEnvironmentVariable("JwtOptions__PrivateKey");

        services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey ?? "testPrivateKey")),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        ConfigureExternalSearcher(services);

        services.AddSingleton(tokenValidationParameters);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString,
                x => x.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name)));

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(PublicMappingProfile).Assembly);
        services.AddAutoMapper(typeof(DomainMappingProfile).Assembly);

        services.AddTransient<DbContext, AppDbContext>();
        services.AddScoped<IBuildingObjectsRepository, BuildingObjectsRepository>();
        services.AddScoped<IBuildingsRepository, BuildingsRepository>();
        services.AddScoped<IEdgesRepository, EdgesRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IFloorsRepository, FloorsRepository>();
        services.AddScoped<IBuildingRoutesService, BuildingRoutesService>();
        services.AddScoped<IBuildingObjectsService, BuildingObjectsService>();
        services.AddScoped<IBuildingsService, BuildingsService>();
        services.AddScoped<IEdgesService, EdgesService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IFloorsService, FloorsService>();
        services.AddScoped<IFloorsBuilder, FloorsBuilder>();

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
        services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        services.AddScoped<IRouteSearcher, RouteSearcher>();

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
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

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        app.UseRouting();

        app.UseAuthorization();
        app.UseCors("CorsPolicy");

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}