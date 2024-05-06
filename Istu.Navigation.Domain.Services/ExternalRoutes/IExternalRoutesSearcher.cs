using Istu.Navigation.Domain.Models.ExternalRoutes;

namespace Istu.Navigation.Domain.Services.ExternalRoutes;

public interface IExternalRoutesSearcher
{
    public Task<ExternalRoute> FindRoute(ExternalPoint startPoint, ExternalPoint endPoint); 
}

public class ExternalRoutesSearcher : IExternalRoutesSearcher
{
    public Task<ExternalRoute> FindRoute(ExternalPoint startPoint, ExternalPoint endPoint)
    {
        throw new NotImplementedException();
    }
}