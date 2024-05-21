namespace Istu.Navigation.Infrastructure.EF.Filters;

public class CommentFilter : BaseFilter
{
    public Guid? CommentId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ObjectId { get; set; }
}