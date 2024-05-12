namespace Istu.Navigation.Infrastructure.EF.Filters;

public class BaseFilter
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 100;
}