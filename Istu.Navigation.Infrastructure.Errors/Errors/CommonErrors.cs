namespace Istu.Navigation.Infrastructure.Errors.Errors;

public class CommonErrors : ErrorBase 
{
    protected new string Nid = CommonConstans.CommonApiNid;
    
    public static ApiError InternalServerError()
    {
        return new ApiError(500, "Произошла внутренняя ошибка сервера.", GetUrn("internal-server-error"));
    }
}