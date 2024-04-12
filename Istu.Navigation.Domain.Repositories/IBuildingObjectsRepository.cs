﻿using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingObjectsRepository : IRepository<BuildingObjectEntity>
{
    public Task<List<BuildingObjectEntity>> GetAllByFilterAsync(BuildingObjectFilter filter);
}

public class BuildingObjectsRepository : Repository<BuildingObjectEntity>, IBuildingObjectsRepository
{
    public BuildingObjectsRepository(DbContext context) : base(context)
    {
    }

    public Task<List<BuildingObjectEntity>> GetAllByFilterAsync(BuildingObjectFilter filter)
    {
        var query = DbSet.AsQueryable();

        if (filter.BuildingId.HasValue)
            query = query.Where(e => e.Id == filter.BuildingId.Value);

        if (filter.Id.HasValue)
            query = query.Where(e => e.Id == filter.Id.Value);

        if (!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(e => e.Title == filter.Title);

        if (filter.Floor.HasValue)
            query = query.Where(e => e.Floor == filter.Floor.Value);

        if (filter.Types != null && filter.Types.Any())
            query = query.Where(e => filter.Types.Contains(e.Type.ToString()));

        query = query.Skip(filter.Skip).Take(filter.Take);

        return query.ToListAsync();
    }
}