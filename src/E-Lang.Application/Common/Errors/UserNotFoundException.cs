using System.Net;

namespace E_Lang.Application.Common.Errors
{
    public class UserNotFoundException : CustomException
    {
        public UserNotFoundException() : base(HttpStatusCode.Unauthorized)
        {
        }

        public override ApiException ToApiException()
        {
            return new ApiException((int)StatusCode, "User not found.");
        }
    }
}
