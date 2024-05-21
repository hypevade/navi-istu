namespace Istu.Navigation.Domain.Models.Entities.Cards;

public class CommentWithUserEntity
{
    public Guid CommentId { get; set; }
    public Guid CreatorId { get; set; }
    public Guid ObjectId { get; set; }
    public DateTime CreationDate { get; set; }
    public string Text { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}