
namespace Istu.Navigation.Domain.Models.Cards;

public class Card
{
    public Guid ObjectId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Address { get; set; }
    public ContentType ContentType { get; set; }
    public List<Guid> ImageIds { get; set; } = new();
    public Dictionary<string, string> Properties { get; set; } = new();
}