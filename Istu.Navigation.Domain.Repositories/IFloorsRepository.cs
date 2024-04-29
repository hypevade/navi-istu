using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IFloorsRepository : IRepository<FloorEntity>
{
    public Task<List<FloorEntity>> GetAllByBuildingIdAsync(Guid buildingId, int minFloor = 1, int maxFloor = int.MaxValue);
    public Task<FloorEntity?> GetByBuildingIdAsync(Guid buildingId, int floorNumber);
}

public class FloorsRepository : Repository<FloorEntity>, IFloorsRepository
{
    public FloorsRepository(DbContext context) : base(context)
    {
    }

    public Task<List<FloorEntity>> GetAllByBuildingIdAsync(Guid buildingId, int minFloor = 1, int maxFloor = int.MaxValue)
    {
        return DbSet.Where(e => e.BuildingId == buildingId && e.FloorNumber >= minFloor && e.FloorNumber <= maxFloor).ToListAsync();
    }

    public async Task<FloorEntity?> GetByBuildingIdAsync(Guid buildingId, int floorNumber)
    {
        var floor = await DbSet.FirstOrDefaultAsync(e => e.BuildingId == buildingId && e.FloorNumber == floorNumber)
            .ConfigureAwait(false);
        return floor;
    }
}
