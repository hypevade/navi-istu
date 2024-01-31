using Istu.Navigation.Domain.Models;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingsRepository
{
    public Building[] GetAll();
    public Building[] GetAllByTitle(string buildingTitle);
    public InnerObject GetById(Guid buildingId);
}