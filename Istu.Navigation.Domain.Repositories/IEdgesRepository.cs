using Istu.Navigation.Domain.Models.InnerObjects;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IEdgesRepository
{
    public Task CreateEdge(Edge edge);
    public Task<OperationResult<List<Edge>>> GetAllByBuildingId(Guid buildingId);
    public Task<OperationResult<Edge>> GetById(Guid edgeId);
    public Task DeleteEdge(Edge edge);
    public Task<OperationResult<List<Edge>>> GetAllByFloor(Guid buildingId, int floor);
}