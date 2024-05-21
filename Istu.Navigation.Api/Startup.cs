using System.Text;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Middlewares;
using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.ExternalRoutes;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Domain.Repositories.Cards;
using Istu.Navigation.Domain.Repositories.Users;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Domain.Services.BuildingRoutes;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Domain.Services.Cards;
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
using Microsoft.OpenApi.Models;

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
        var filename = Configuration.GetSection("Map").GetSection("FileName").Value ?? string.Empty;
        var mapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

        var routerDb = new RouterDb();
        using (var stream = new FileInfo(mapPath).OpenRead())
        {
            routerDb.LoadOsmData(stream, Vehicle.Pedestrian, Vehicle.Bicycle);
        };
        services.Configure<MapOptions>(Configuration.GetSection("Map"));

        services.AddSingleton(routerDb);
        services.AddScoped<Router>();
        services.AddScoped<IExternalRoutesSearcher, ExternalRoutesSearcher>();
    }

    private void ConfigureOAuth(IServiceCollection services)
    {
        var clientSecret = Environment.GetEnvironmentVariable("OAuth__ClientSecret");
        services.Configure<OAuthOptions>(Configuration.GetSection("OAuth") );
        if (!string.IsNullOrEmpty(clientSecret))
        {
            services.PostConfigure<OAuthOptions>(options =>
            {
                options.ClientSecret = clientSecret;
            });
        }
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
            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero
        };
        
        ConfigureExternalSearcher(services);
        ConfigureOAuth(services);

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

        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddAutoMapper(typeof(PublicMappingProfile).Assembly);
        services.AddAutoMapper(typeof(DomainMappingProfile).Assembly);

        services.AddTransient<DbContext, AppDbContext>();
        services.AddScoped<IBuildingObjectsService, BuildingObjectsService>();
        services.AddScoped<IBuildingObjectsRepository, BuildingObjectsRepository>();

        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IFileStorage, FileStorage>();
        services.AddScoped<IImageRepository, ImageRepository>();

        services.AddScoped<IBuildingsService, BuildingsService>();
        services.AddScoped<IBuildingsRepository, BuildingsRepository>();

        services.AddScoped<IEdgesRepository, EdgesRepository>();
        services.AddScoped<IEdgesService, EdgesService>();

        services.AddScoped<IFloorsService, FloorsService>();
        services.AddScoped<IFloorsBuilder, FloorsBuilder>();
        services.AddScoped<IFloorsRepository, FloorsRepository>();

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
        services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        services.AddScoped<ILuceneService, LuceneService>();

        services.AddScoped<ICommentsRepository, CommentsRepository>();
        services.AddScoped<ICommentsService, CommentsService>();
        
        services.AddScoped<IBuildingRoutesService, BuildingRoutesService>();

        services.AddScoped<IRouteSearcher, RouteSearcher>();

        services.AddScoped<IIstuService, IstuService>();

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationModelErrorWrapFilter>(-2001);
        });
        services.AddHttpClient();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseMiddleware<RequestLoggingMiddleware>();
        
        //TODO: отключить свагер UI для прода
        /*if (env.IsDevelopment())
        {*/
            app.UseDeveloperExceptionPage();
            app.UseSwaggerUI();
        /*}*/

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