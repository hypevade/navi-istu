using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingsRepository : IRepository<BuildingEntity>
{
    public Task<IEnumerable<BuildingEntity>> GetAllByTitle(string title);
}

public class BuildingsRepository : Repository<BuildingEntity> , IBuildingsRepository
{
    public BuildingsRepository(DbContext context) : base(context)
    {}

    public async Task<IEnumerable<BuildingEntity>> GetAllByTitle(string title)
    {
        return await DbSet.Where(building => building.Title == title).ToListAsync().ConfigureAwait(false);
    }
}