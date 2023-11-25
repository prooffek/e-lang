using System.Net;

namespace E_Lang.Application.Common.Errors
{
    public abstract class CustomValidationException : CustomException
    {
        public string EntityName { get; set; }

        protected CustomValidationException(string entityName, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(statusCode)
        {
            EntityName = entityName;
        }
    }
}
