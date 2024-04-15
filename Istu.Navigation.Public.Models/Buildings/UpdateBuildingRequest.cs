using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Buildings;

public class UpdateBuildingRequest
{
    [Required]
    public Guid Id { get; set; }
    
    public string? UpdatedTitle { get; set; }
    public string? UpdatedDescription { get; set; }
    public string? UpdatedAddress { get; set; }
    
    public int? UpdatedFloorNumbers { get; set; }
    public double? UpdatedLatitude { get; set; }
    public double? UpdatedLongitude { get; set; }
}