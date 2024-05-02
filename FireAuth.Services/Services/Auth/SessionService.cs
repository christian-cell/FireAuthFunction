using FireAuth.Domain.Entities;
using FireAuth.Domain.Infraestructure;
using FireAuth.Models.Configurations;
using FireAuth.Models.Models;
using FireAuth.Repository;
using Microsoft.EntityFrameworkCore;
using FireAuth.Services.Abstractions.Auth;

namespace FireAuth.Services.Services.Auth
{
    public class SessionService : EntityRepository , ISessionService
    {
    private TokenConfiguration _tokenConfiguration { get; }
        
        public SessionService(
            TokenConfiguration configuration,
            UserDbContext dbContext
            ):base(dbContext)
        {
            _tokenConfiguration = configuration;
        }
        
        public async Task UpdateSessionAsync( UserSession session )
        {
            bool ok = await UpdateAsync<UserSession>(session.Id, session).ConfigureAwait(false);
            
            if (ok) ok = await SaveAsync().ConfigureAwait(false);

            if (!ok)
            {
                throw new Exception("Something went wrong updating Session");
            }
        }
        
        public bool CheckIfRefreshTokenExpired(UserSession session)
        {
            return DateTime.UtcNow > session.RefreshTokenExpirationDate || session.Used ? true : false;
        }
        
        public async Task<UserSession> GetLastUserSessionAsync(string? refreshToken, Guid? customerId)
        {
            var sessions = await SearchAsync<UserSession>(0, 1, cs => 
                cs.RefreshToken == refreshToken && cs.Deleted == false);
            
            var session = await sessions.OrderByDescending(session => session.CreatedOn).FirstOrDefaultAsync();

            return session;
        }
        
        public async Task<bool> InsertNewSessionAsync(User user , string newAccessToken)
        {
            var newSession = new UserSession()
            {
                UserId = user.Id,
                RefreshToken = newAccessToken,
                RefreshTokenExpirationDate = DateTime.UtcNow.AddSeconds(_tokenConfiguration.RefreshTokenExpiration),
                Used = false
            };

            await AddAsync<UserSession>(newSession);
            if (await SaveAsync())
            {
                return true;
            }
            else
            {
                throw new Exception("Failed to save new session");
            }
        }
    }
};

