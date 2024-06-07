using Istu.Navigation.Domain.Models.Entities.Cards;
using Istu.Navigation.Domain.Repositories.Base;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.EF.Filters;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories.Cards;

public interface ICommentsRepository : IRepository<CommentEntity>
{
    Task<List<CommentEntity>> GetAllByFilter(CommentFilter filter);
    Task<List<CommentWithUserEntity>> GetCommentsWithUsersByFilterAsync(CommentFilter filter);
}

public class CommentsRepository(AppDbContext context) : Repository<CommentEntity>(context), ICommentsRepository
{
    public Task<List<CommentEntity>> GetAllByFilter(CommentFilter filter)
    {
        var query = DbSet.AsQueryable();
        if (filter.ObjectId.HasValue)
            query = query.Where(e => e.ObjectId == filter.ObjectId.Value);

        if (filter.CommentId.HasValue)
            query = query.Where(e => e.Id == filter.CommentId.Value);

        if (filter.UserId.HasValue)
            query = query.Where(e => e.CreatorId == filter.UserId.Value);

        query = query.OrderBy(x=>x.CreationDate).Skip(filter.Skip).Take(filter.Take);
        return query.ToListAsync();
    }
    
    public Task<List<CommentWithUserEntity>> GetCommentsWithUsersByFilterAsync(CommentFilter filter)
    {
        var query = from comment in DbSet
            join user in Context.Users on comment.CreatorId equals user.Id
            select new CommentWithUserEntity
            {
                CommentId = comment.Id,
                CreatorId = comment.CreatorId,
                CreationDate = comment.CreationDate,
                Text = comment.Text,
                ObjectId = comment.ObjectId,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        if (filter.ObjectId.HasValue)
            query = query.Where(c => c.ObjectId == filter.ObjectId.Value);
        
        if (filter.CommentId.HasValue)
            query = query.Where(c => c.CommentId == filter.CommentId.Value);
        
        if (filter.UserId.HasValue)
            query = query.Where(c => c.CreatorId == filter.UserId.Value);
        
        query = query.OrderBy(x=>x.CreationDate).Skip(filter.Skip).Take(filter.Take);
        return query.ToListAsync();
    }
}