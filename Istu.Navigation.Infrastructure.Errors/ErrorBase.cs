namespace Istu.Navigation.Infrastructure.Errors;

public abstract class ErrorBase
{
    private static readonly string UrnPrefix = CommonConstans.Urn;
    
    protected abstract string Nid { get; }
    
    protected string GetUrn(string nss)
    {
        return $"{UrnPrefix}:{Nid}:{nss}";
    }
}
