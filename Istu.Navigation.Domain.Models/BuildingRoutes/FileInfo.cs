namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class FileInfo(string name, byte[] content)
{
    public string Name { get; set; } = name;
    public byte[] Content { get; set; } = content;
}