using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FireAuth.Models.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FireAuth.Models.Configurations;

namespace GoodDeal.API.Auth
{
    public class Jwt
    {
        internal static void ConfigureJwtService(JwtBearerOptions options, TokenConfiguration configuration)
        {
            var keyByteArray = Encoding.UTF8.GetBytes(configuration.Key);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration.Issuer,
                ValidAudience = configuration.Audience,
                IssuerSigningKey = signingKey
            };

            // By default, certain .NET libraries apply automatic transformations according to standards. Consequently, the claim "email" is automatically translated to
            // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress, when it should remain as "email."
            options.SecurityTokenValidators.Clear();
            options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler()
            {
                InboundClaimTypeMap = new Dictionary<string, string>()
                {
                    { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "email" }
                }
            });

        }
    }
};

