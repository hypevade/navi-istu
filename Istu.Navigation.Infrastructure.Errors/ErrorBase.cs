namespace Istu.Navigation.Infrastructure.Errors;

public abstract class ErrorBase
{
    private static readonly string UrnPrefix = CommonConstans.Urn;

    protected static string Nid;
    
    protected static string GetUrn(string nss)
    {
        return $"{UrnPrefix}:{Nid}:{nss}";
    }
}
