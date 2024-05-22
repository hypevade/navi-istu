using Istu.Navigation.Domain.Models.Cards;

namespace Istu.Navigation.Public.Models.Cards;

public class SearchResultDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required ContentType Type { get; set; }
}