using Istu.Navigation.Domain.Models;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingsRepository
{
    public Task<OperationResult<List<Building>>> GetAll();
    public Task<OperationResult<List<Building>>> GetAllByTitle(string buildingTitle);
    public Task<OperationResult<Building>> GetById(Guid buildingId);
}