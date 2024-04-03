using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Domain.Services;
using Moq;

namespace Istu.Navigation.UnitTests.BuildingRoutes;

public class BuildingRoutesTests
{
    private IBuildingRoutesService routesService;

    [SetUp]
    public void Setup()
    {
        var buildingObjectRepository = new Mock<IBuildingObjectsRepository>();
        var buildingsRepository = new Mock<IBuildingsRepository>();
        var floorsRepository = new Mock<IFloorsRepository>();
        var routeSearcher = new Mock<IRouteSearcher>();
        
        routesService = new BuildingRoutesService(buildingObjectRepository.Object,
            buildingsRepository.Object,
            floorsRepository.Object,
            routeSearcher.Object);
    }

    [Test]
    public void Should_return_correct_route_when_two_obj()
    {
        
    }
    
}