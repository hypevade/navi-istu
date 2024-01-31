using Istu.Navigation.Domain.Models;
using Istu.Navigation.Public.Models;
using Route = Istu.Navigation.Domain.Models.Route;

namespace Istu.Navigation.Api.Converters;

public interface IRoutesConverter
{
    public PublicRoute ConvertToPublicRoute(List<(Floor, Route)> internalRoute); 
}