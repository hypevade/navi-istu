using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Buildings;

public class CreateBuildingRequest
{
    [Required]
    public required string Title { get; set; }
    
    public required string Address { get; set; }
    public string? Description { get; set; }
    
    public string? Keywords { get; set; }
    public required ExternalPositionDto ExternalPosition { get; set; }
}