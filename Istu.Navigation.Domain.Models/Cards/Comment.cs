using Istu.Navigation.Domain.Models.Entities.Cards;

namespace Istu.Navigation.Domain.Models.Cards;

public class Comment
{
    public Comment(Guid id, Guid creatorId, string text, DateTime creationDate, string creatorName)
    {
        Id = id;
        CreatorId = creatorId;
        Text = text;
        CreationDate = creationDate;
        CreatorName = creatorName;
    }

    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Text { get; set; }
    public DateTime CreationDate { get; set; }
    public string CreatorName { get; set; }

    public static Comment FromEntity(CommentEntity entity, string firstName, string lastName)
    {
        var creatorName = $"{firstName} {lastName}";
        return new Comment(entity.Id, entity.CreatorId, entity.Text, entity.CreationDate, creatorName);
    }
}