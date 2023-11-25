using E_Lang.Application.Common.Enums;

namespace E_Lang.Application.Common.Errors
{
    public class NullOrEmptyValidationException : CustomValidationException
    {
        public string PropertyName { get; set; }
        public ActionTypes ActionType { get; set; }

        public NullOrEmptyValidationException(string entityName, string propertyName, ActionTypes actionType) : base(entityName)
        {
            PropertyName = propertyName;
            ActionType = actionType;
        }

        public override ApiException ToApiException()
        {
            var message = $"{Enum.GetName(typeof(ActionTypes), ActionType)} record: Validation failed - {PropertyName} cannot be null or empty.";
            return new ApiException((int)StatusCode, message);
        }
    }
}
