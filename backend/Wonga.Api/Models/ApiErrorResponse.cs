namespace Wonga.Api.Models;

public record ApiErrorResponse(
    string Code,
    string Message,
    IDictionary<string, string[]>? Errors = null)
{
    public static ApiErrorResponse Validation(IDictionary<string, string[]> errors) =>
        new("validation_error", "Request validation failed.", errors);

    public static ApiErrorResponse Create(string code, string message) =>
        new(code, message);
}
