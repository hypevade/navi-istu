using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.EF.Filters;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Buildings;

public interface IBuildingsRepository : IRepository<BuildingEntity>
{
    public Task<List<BuildingEntity>> GetAllByFilterAsync(BuildingFilter filter);
    public Task<bool> ExistWithTitle(string title);
}

public class BuildingsRepository : Repository<BuildingEntity> , IBuildingsRepository
{
    public BuildingsRepository(AppDbContext context) : base(context)
    {}
    
    public Task<List<BuildingEntity>> GetAllByFilterAsync(BuildingFilter filter)
    {
        var query = DbSet.AsQueryable();

        if (filter.BuildingId.HasValue)
        {
            query = query.Where(e => e.Id == filter.BuildingId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            query = query.Where(e=> e.Title == filter.Title);
        }
        
        query = query.Skip(filter.Skip).Take(filter.Take);

        return query.ToListAsync();
    }

    public Task<bool> ExistWithTitle(string title)
    {
        return DbSet.AnyAsync(e => e.Title.ToLower() == title.ToLower());
    }
}