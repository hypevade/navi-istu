using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public class BuildingsRepository : IBuildingsRepository
{
    private readonly BuildingsDbContext dbContext;
    private readonly IMapper mapper;

    public BuildingsRepository(BuildingsDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<OperationResult<List<Building>>> GetAll(int take = 100, int skip = 0)
    {
        var buildings = await dbContext.Buildings.Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);
        return OperationResult<List<Building>>.Success(mapper.Map<List<Building>>(buildings));
    }

    public async Task<OperationResult<List<Building>>> GetAllByTitle(string buildingTitle, int take = 100 , int skip = 0)
    {
        if(string.IsNullOrWhiteSpace(buildingTitle))
            throw new ArgumentException("buildingTitle cannot be null or empty", nameof(buildingTitle));
        var buildings = await dbContext.Buildings.Where(x =>
                string.Equals(x.Title.ToLower(), buildingTitle.ToLower(), StringComparison.Ordinal)).Skip(skip)
            .Take(take)
            .ToListAsync().ConfigureAwait(false);
        if (!buildings.Any())
            return OperationResult<List<Building>>.Failure(
                BuildingRoutesErrors.BuildingsWithTitleNotFoundError(buildingTitle));
        
        return OperationResult<List<Building>>.Success(mapper.Map<List<Building>>(buildings));
    }

    public async Task<OperationResult<Building>> GetById(Guid buildingId)
    {
        var building = await dbContext.Buildings.FirstOrDefaultAsync(x => x.Id == buildingId);
        
        return building is null
            ? OperationResult<Building>.Failure(BuildingRoutesErrors.BuildingWithIdNotFoundError(buildingId))
            : OperationResult<Building>.Success(mapper.Map<Building>(building));
    }

    public Task<OperationResult> CreateBuildings(List<Building> buildings)
    {
        throw new NotImplementedException();
    }
}