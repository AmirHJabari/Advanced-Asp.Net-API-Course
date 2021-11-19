using Common.Utilities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Common
{
    public static class IdentityExtensions
    {
        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity?.FindFirst(claimType)?.Value;
        }

        public static ApiResultStatusCode GetApiResultCode(this IdentityResult result)
        {
            ApiResultStatusCode code = ApiResultStatusCode.None;
            
            if (!result.Succeeded)
                code = ApiResultStatusCode.InvalidInputs;

            if (result.Errors.Count() == 1)
            {
                if (result.Errors.First().Code.StartsWith("Password", StringComparison.OrdinalIgnoreCase))
                    return ApiResultStatusCode.WeakPassword;

                switch (result.Errors.First().Code)
                {
                    case "DuplicateUserName":
                        code = ApiResultStatusCode.DuplicateUserName;
                        break;
                    case "DuplicateEmail":
                        code = ApiResultStatusCode.DuplicateEmail;
                        break;
                }
            }

            return code;
        }
        public static string GetErrorMessages(this IdentityResult result)
        {
            if (result.Succeeded)
                return null;

            return string.Join('|', result.Errors.Select(e => e.Description));
        }

        public static string FindFirstValue(this IIdentity identity, string claimType)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity?.FindFirstValue(claimType);
        }

        public static string GetUserId(this IIdentity identity)
        {
            return identity?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            var userId = identity?.GetUserId();
            return userId.HasValue()
                ? (T)Convert.ChangeType(userId, typeof(T), CultureInfo.InvariantCulture)
                : default(T);
        }

        public static string GetUserName(this IIdentity identity)
        {
            return identity?.FindFirstValue(ClaimTypes.Name);
        }
    }
}
