using System.Net;

namespace E_Lang.Application.Common.Errors;

public class ApiException
{
    public int StatusCode { get; set; }
    public string? StatusText { get; set; }
    public string? Message { get; set; } = null;
    public string? Details { get; set; } = null;

    public ApiException(int statusCode, string message, string? details = null)
    {
        StatusCode = statusCode;
        StatusText = Enum.GetName(typeof(HttpStatusCode), StatusCode);
        Message = message;
        Details = details;
    }
}