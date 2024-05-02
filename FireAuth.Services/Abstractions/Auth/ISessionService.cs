using FireAuth.Domain.Entities;

namespace FireAuth.Services.Abstractions.Auth
{
    public interface ISessionService
    {
        Task UpdateSessionAsync(UserSession session);
        bool CheckIfRefreshTokenExpired(UserSession session);
        Task<UserSession?> GetLastUserSessionAsync(string? refreshToken, Guid? userId);
        Task<bool> InsertNewSessionAsync(User user, string newAccessToken);
    }
};

