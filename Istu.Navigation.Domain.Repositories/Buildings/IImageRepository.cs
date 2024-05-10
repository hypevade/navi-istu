using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF.Filters;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Buildings;

public interface IImageRepository : IRepository<ImageInfoEntity>
{
    public Task<List<ImageInfoEntity>> GetAllByObjectId(Guid objectId);
    public Task<List<ImageInfoEntity>> GetAllByFilterAsync(ImageFilter filter);
}

public class ImageRepository(DbContext context) : Repository<ImageInfoEntity>(context), IImageRepository
{
    public Task<List<ImageInfoEntity>> GetAllByObjectId(Guid objectId)
    {
        return DbSet.Where(x => x.ObjectId == objectId).ToListAsync();
    }
    
    public Task<List<ImageInfoEntity>> GetAllByFilterAsync(ImageFilter filter)
    {
        var query = DbSet.AsQueryable();

        if (filter.ObjectId.HasValue)
            query = query.Where(e => e.ObjectId == filter.ObjectId.Value);
        
        if (filter.ImageId.HasValue)
            query = query.Where(e => e.Id == filter.ImageId.Value);
        
        query = query.Skip(filter.Skip).Take(filter.Take);

        return query.ToListAsync();
    }
}