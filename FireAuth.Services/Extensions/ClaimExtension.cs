using FireAuth.Models.Models;
using Microsoft.AspNetCore.Http;
using FireAuth.Models.Models;

namespace FireAuths.Services.Extensions
{
    public static class ClaimExtension
    {
        public static string GetValueFromUserClaims<T>(HttpContext context, UserClaim userClaim)
        {
            return context.User.Claims.FirstOrDefault()?.Value;
        }
    }
};