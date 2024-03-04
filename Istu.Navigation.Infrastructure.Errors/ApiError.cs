namespace Istu.Navigation.Infrastructure.Errors;

public class ApiError
{
    public int StatusCode { get; set; } // HTTP статус код
    public string Message { get; set; } // Сообщение об ошибке

    public string Urn { get; set; } // HTTP статус код

    public ApiError(int statusCode, string message, string urn)
    {
        StatusCode = statusCode;
        Message = message;
        Urn = urn;
    }
}
