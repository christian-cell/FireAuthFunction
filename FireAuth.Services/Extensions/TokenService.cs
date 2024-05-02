using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FireAuth.Domain.Entities;
using FireAuth.Domain.Infraestructure;
using FireAuth.Models.Configurations;
using FireAuth.Models.Exceptions;
using FireAuth.Models.Responses;
using FireAuth.Repository;
using FireAuth.Services.Abstractions.Auth;
using Microsoft.IdentityModel.Tokens;

namespace FireAuth.Services.Extensions
{
    public class TokenService : EntityRepository , ITokenService
    {
    private TokenConfiguration _tokenConfiguration { get; }
        private readonly ISessionService _sessionService;

        public TokenService(
            ISessionService sessionService,
            TokenConfiguration configuration,
            UserDbContext dbContext
            ):base(dbContext)
        {
            _sessionService = sessionService;
            _tokenConfiguration = configuration;
        }

        public string GenerateToken(IDictionary<string, string> properties)
        {
            var keyByteArray = Encoding.UTF8.GetBytes(_tokenConfiguration.Key);

            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var expiration = DateTime.UtcNow.AddSeconds(_tokenConfiguration.Expiration);

            var claims = new List<Claim>();

            foreach (var property in properties)
            {
                claims.Add(new Claim(property.Key, property.Value));
            }

            var token = new JwtSecurityToken(
                issuer: _tokenConfiguration.Issuer,
                audience: _tokenConfiguration.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        
        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {

            var session = await _sessionService.GetLastUserSessionAsync(refreshToken, null);

            if (session == null)
            {
                throw new NotFoundException("UserSession" , refreshToken); 
            }
            else
            {
                var user = await FindAsync<User>(session.UserId);

                if (user == null)
                {
                    throw new NotFoundException("User" , user.Id);  
                }
            
                if(_sessionService.CheckIfRefreshTokenExpired(session))throw new UnauthorizedAccessException();
            
                var token = GenerateToken(new Dictionary<string, string>()
                {
                    { "id", user.Id.ToString() },
                    { "email", user.Email },
                    { "phone", user.Phone },
                    { "firstName", user.FirstName },
                    { "lastName", user.LastName },
                    { "documentNumber", user.DocumentNumber }
                });
            
                return new AuthResponse()
                {
                    Token = token,
                    Lifetime = _tokenConfiguration.Expiration,
                    Expiration = DateTime.UtcNow.AddSeconds(_tokenConfiguration.Expiration),
                    MD5 = GetMD5(token),
                    UserId = user.Id
                };   
            }
        }
        
        public string GetMD5(string @string)
        {

            var bytes = Encoding.UTF8.GetBytes(@string);

            using var md5 = MD5.Create();

            var hashBytes = md5.ComputeHash(bytes);

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
};

