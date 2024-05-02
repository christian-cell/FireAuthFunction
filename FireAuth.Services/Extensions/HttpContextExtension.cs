using System.ComponentModel;
using System.Globalization;
using FireAuth.Models.Models;
using FireAuth.Models.Models;
using Microsoft.AspNetCore.Http;

namespace FireAuth.Services.Extensions
{
    public static class HttpContextExtension
    {
        public static T GetValueFromClaimsPrincipal<T>(this HttpContext context, UserClaim userClaim)
        {
            if (context == null) throw new Exception("HttpContext is null");

            if (!context.User.Claims.Any()) return default;

            var claim = context.User.Claims
                .ToList()
                .FirstOrDefault(x => x.Type == userClaim.GetEnumDescriptionAttribute());

            if (claim == null) return default;

            var converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, claim.Value);

        }

    }
};

