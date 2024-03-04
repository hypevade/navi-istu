namespace Istu.Navigation.Infrastructure.Errors;

public class OperationResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    // Фабричные методы для создания результатов
    public static OperationResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static OperationResult<T> Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };
}
