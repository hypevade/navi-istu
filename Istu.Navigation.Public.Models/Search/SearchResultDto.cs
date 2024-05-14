using Istu.Navigation.Domain.Services;

namespace Istu.Navigation.Public.Models.Search;

public class SearchResultDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required ContentType Type { get; set; }
}