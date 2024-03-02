using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingsRepository
{
    public Building[] GetAll();
    public Building[] GetAllByTitle(string buildingTitle);
    public InnerObject GetById(Guid buildingId);
}