using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Services;

public interface IRouteSearcher
{
    public Task<(Floor Floor, Route Route)> CreateRoute(InnerObject fromInnerObject, InnerObject toInnerObject, Floor floor);
    public Task<(Floor Floor, Route Route)> GetRoute(Guid routeI);
}