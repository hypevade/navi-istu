namespace Istu.Navigation.Public.Models;

public class ErrorDto
{
    public required string Message { get; set; }
    public required string Urn { get; set; }
    public required int StatusCode { get; set; }
}