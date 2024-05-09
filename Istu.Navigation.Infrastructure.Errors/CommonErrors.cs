using System.Text;

namespace Istu.Navigation.Infrastructure.Errors;

public class CommonErrors 
{
    private static string GetUrn(string errorType) => $"urn:common-errors:{errorType}";
    
    public static ApiError InternalServerError()
    {
        return new ApiError(500, "Произошла внутренняя ошибка сервера.", GetUrn("internal-server-error"));
    }
    
    public static ApiError EmptyTitleError()
    {
        return new ApiError(400,
            "Свойство названия не может быть пустым.",
            GetUrn("title-is-empty"));
    }
    
    public static ApiError FileTooLarge(long fileSize, long maxFileSize)
    {
        return new ApiError(400,
            $"Файл слишком большой. Размер: {fileSize} бит. Максимальный размер: {maxFileSize} бит.",
            GetUrn("file-too-large"));
    }
    public static ApiError ValidationModelError(Dictionary<string, string[]> errors)
    {
        return new ApiError(400,
            $"Ошибки валидации: {FormatErrors(errors)}",
            GetUrn("validation-model-error"));
    }
    
    
    public static string FormatErrors(Dictionary<string, string[]> errors)
    {
        var builder = new StringBuilder();
        
        foreach (var error in errors)
        {
            builder.Append($"{error.Key}: ");
            builder.Append(string.Join(", ", error.Value));
        }
        
        return builder.ToString();
    }
}