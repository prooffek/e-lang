using System.Net;

namespace E_Lang.Application.Common.Errors
{
    public class NotFoundValidationException : CustomValidationException
    {
        public string AttributeName { get; set; }
        public string Value { get; set; }

        public NotFoundValidationException(string entityName, string attributeName, string value) : base(entityName)
        {
            AttributeName = attributeName;
            Value = value;
        }

        public override ApiException ToApiException()
        {
            var message = $"{EntityName} with {AttributeName} {Value} not found.";
            return new ApiException((int)StatusCode, message);
        }
    }
}
