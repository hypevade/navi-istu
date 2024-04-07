namespace Istu.Navigation.TestClient;

public class IstuNavigationTestClient
{
    private IstuNavigationTestClient(IRoutesClient routesClient, IBuildingsClient buildingsClient)
    {
        RoutesClient = routesClient;
        BuildingsClient = buildingsClient;
    }
    public IRoutesClient RoutesClient { get; init; }
    public IBuildingsClient BuildingsClient { get; init; }

    public static IstuNavigationTestClient Create(HttpClient? client = null)
    {
        client ??= new HttpClient();
        var routesClient = new RoutesClient(client);
        var buildingsClient = new BuildingsClient(client);
        return new IstuNavigationTestClient(routesClient, buildingsClient);
    }
}
