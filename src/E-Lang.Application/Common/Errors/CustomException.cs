using System.Net;

namespace E_Lang.Application.Common.Errors;

public abstract class CustomException : Exception
{
    public HttpStatusCode StatusCode { get; }

    protected CustomException(HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        StatusCode = statusCode;
    }
    
    public abstract ApiException ToApiException();
}