namespace Istu.Navigation.Infrastructure.EF.Filters;

public class ImageFilter : BaseFilter
{
    public Guid? ObjectId { get; set; }
    public Guid? ImageId { get; set; } 
    public string? Title { get; set; } 
    public string? Link { get; set; } 
}