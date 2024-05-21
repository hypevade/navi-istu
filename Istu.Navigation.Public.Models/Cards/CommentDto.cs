namespace Istu.Navigation.Public.Models.Cards;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public string CreatorName { get; set; } = string.Empty;
}