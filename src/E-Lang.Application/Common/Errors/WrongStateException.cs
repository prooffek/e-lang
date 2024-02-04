using E_Lang.Domain.Enums;
using System.Net;

namespace E_Lang.Application.Common.Errors;

public class WrongStateException : CustomException
{
    public string ExpectedStatus { get; set; }
    public string CurrentStatus { get; set; }
    public string EntityName { get; set; }

    public WrongStateException(FlashcardStatus expectedStatus, FlashcardStatus currentStatus, string entityName) : base(HttpStatusCode.InternalServerError)
    {
        ExpectedStatus = Enum.GetName(expectedStatus);
        CurrentStatus = Enum.GetName(currentStatus);
        EntityName = entityName;
    }

    public WrongStateException(string expectedStatus, string currentStatus, string entityName) : base(HttpStatusCode.InternalServerError)
    {
        ExpectedStatus = expectedStatus;
        CurrentStatus = currentStatus;
        EntityName = entityName;
    }
    
    public override ApiException ToApiException()
    {
        return new ApiException((int) StatusCode,
            $"Wrong status: {EntityName} should have status {ExpectedStatus}, but has {CurrentStatus}.");
    }
}