using System.Net;

namespace E_Lang.Application.Common.Errors
{
    public class RelatedRecordValidationException : CustomValidationException
    {
        public string Instruction { get; set; }

        public RelatedRecordValidationException(string entityName, string instruction) : base(entityName, HttpStatusCode.BadRequest)
        {
            Instruction = instruction;
        }

        public override ApiException ToApiException()
        {
            var message = $"Cannot delete {EntityName}. {Instruction}";
            return new ApiException((int)StatusCode, message);
        }
    }
}
