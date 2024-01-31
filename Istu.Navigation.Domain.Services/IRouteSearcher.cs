using Istu.Navigation.Domain.Models;

namespace Istu.Navigation.Domain.Services;

public interface IRouteSearcher
{
    public Task<(Floor Floor, Route Route)> SearchRoute(InnerObject fromInnerObject, InnerObject toInnerObject, Floor floor);
}