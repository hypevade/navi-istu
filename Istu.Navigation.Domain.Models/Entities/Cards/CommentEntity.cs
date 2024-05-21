namespace Istu.Navigation.Domain.Models.Entities.Cards;

public class CommentEntity: BaseEntity
{
    public Guid ObjectId { get; set; }
    public Guid CreatorId { get; set; }
    public string Text { get; set; }
    public DateTime CreationDate { get; set; }
}