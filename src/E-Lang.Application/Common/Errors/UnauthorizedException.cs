using E_Lang.Application.Common.Enums;
using System.Net;

namespace E_Lang.Application.Common.Errors
{
    public class UnauthorizedException : CustomException
    {
        public Guid UserId { get; set; }
        public ActionTypes ActionType { get; set; }

        public UnauthorizedException(Guid userId, ActionTypes actionType) : base(HttpStatusCode.Unauthorized)
        {
            UserId = userId;
            ActionType = actionType;
        }

        public override ApiException ToApiException()
        {
            var message = $"{Enum.GetName(typeof(ActionTypes), ActionType)} - Access denied. User with id ${UserId} does not have necessary permissions.";
            return new ApiException((int)StatusCode, message);
        }
    }
}
