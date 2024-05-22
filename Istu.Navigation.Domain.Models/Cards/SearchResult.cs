namespace Istu.Navigation.Domain.Models.Cards;

public class SearchResult(Guid id, string title, ContentType type)
{
    public Guid Id { get; set; } = id;
    public ContentType Type { get; set; } = type;

    public string Title { get; set; } = title;
}