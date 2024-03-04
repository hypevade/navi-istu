using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingsRepository
{
    public Task<List<Building>> GetAll();
    public Task<List<Building>> GetAllByTitle(string buildingTitle);
    public Task<Building> GetById(Guid buildingId);
}