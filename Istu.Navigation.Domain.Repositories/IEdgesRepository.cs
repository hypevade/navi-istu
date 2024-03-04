using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Repositories;

public interface IEdgesRepository
{
    public Task CreateEdge(Edge edge);
    public Task<List<Edge>> GetAllByBuildingId(Guid buildingId);
    public Task<Edge?> GetById(Guid edgeId);
    public Task DeleteEdge(Edge edge);
    public Task<List<Edge>> GetAllByFloor(Guid buildingId, int floor);
}