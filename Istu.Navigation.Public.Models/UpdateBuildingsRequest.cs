namespace Istu.Navigation.Public.Models;

public class UpdateBuildingsRequest
{
    public required List<BuildingDto> Buildings { get; set; }
}