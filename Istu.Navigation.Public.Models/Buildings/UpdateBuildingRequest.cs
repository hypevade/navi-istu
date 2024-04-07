using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Buildings;

public class UpdateBuildingRequest
{
    [Required]
    public required BuildingDto Building { get; set; }
}