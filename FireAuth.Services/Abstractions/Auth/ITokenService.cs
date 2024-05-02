using FireAuth.Models.Responses;

namespace FireAuth.Services.Abstractions.Auth
{
    public interface ITokenService
    {
        string GenerateToken(IDictionary<string, string> properties);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        string GetMD5(string @string);
        string GenerateRefreshToken();
    }
};

