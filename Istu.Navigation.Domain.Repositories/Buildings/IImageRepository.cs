using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF.Filters;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Buildings;

public interface IImageRepository : IRepository<ImageLinkEntity>
{
    public Task<List<ImageLinkEntity>> GetAllByObjectId(Guid objectId);
    public Task<List<ImageLinkEntity>> GetAllByFilterAsync(ImageFilter filter);
}

public class ImageRepository(DbContext context) : Repository<ImageLinkEntity>(context), IImageRepository
{
    public Task<List<ImageLinkEntity>> GetAllByObjectId(Guid objectId)
    {
        return DbSet.Where(x => x.ObjectId == objectId).ToListAsync();
    }
    
    public Task<List<ImageLinkEntity>> GetAllByFilterAsync(ImageFilter filter)
    {
        var query = DbSet.AsQueryable();

        if (filter.ObjectId.HasValue)
            query = query.Where(e => e.ObjectId == filter.ObjectId.Value);
        
        if (filter.Title != null)
            query = query.Where(e => e.Title == filter.Title);
        
        if (filter.ImageId.HasValue)
            query = query.Where(e => e.Id == filter.ImageId.Value);
        
        if (filter.Link != null)
            query = query.Where(e => e.Link == filter.Link);
        
        query = query.Skip(filter.Skip).Take(filter.Take);

        return query.ToListAsync();
    }
}