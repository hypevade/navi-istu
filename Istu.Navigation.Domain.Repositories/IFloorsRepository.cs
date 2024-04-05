using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IFloorsRepository: IRepository<FloorEntity>
{
    public Task<FloorEntity?> GetById(Guid buildingId, int floorNumber);
    public Task<List<FloorEntity>> GetAllByBuilding(Guid buildingId);
}

public class FloorsRepository : Repository<FloorEntity>, IFloorsRepository
{
    public FloorsRepository(DbContext context) : base(context)
    { }

    public async Task<FloorEntity?> GetById(Guid buildingId, int floorNumber)
    {
        var floor = await DbSet.FindAsync(new { buildingId, floorNumber }).ConfigureAwait(false);
        return floor;
    }

    public async Task<List<FloorEntity>> GetAllByBuilding(Guid buildingId)
    {
        var floors = await DbSet
            .Where(floor => floor.BuildingId == buildingId)
            .ToListAsync()
            .ConfigureAwait(false);
        return floors;
    }
} 