using MediatR;
using FireAuth.Domain.Infraestructure;
using FireAuth.Models.Commands;
using FireAuth.Models.Configurations;
using FireAuth.Models.Responses;
using FireAuth.Repository;
using FireAuth.Services.Abstractions.Auth;

namespace FireAuth.Services.CommandHandlers.Auth
{
    public class AuthCommandHandler : EntityRepository , IRequestHandler<AuthCommand, AuthResponse> 
    {

        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;
        private readonly TokenConfiguration _tokenConfiguration;
            
        public AuthCommandHandler(
            IUserService userService,
            ISessionService sessionService,
            ITokenService tokenService,
            UserDbContext dbContext,
            TokenConfiguration tokenConfiguration
            ):base(dbContext)
        {
            _userService = userService;
            _tokenConfiguration = tokenConfiguration;
            _tokenService = tokenService;
            _sessionService = sessionService;
        }
        
        /*
         * flow : check if customer exists , verify him , generate accessToken , check if refreshTokenExpired
         * if it is : generate a new one , insert new refreshToken at database , update last session as Used
         * if it is not : using vigent refreshToken
         * at the end return a response with whatever refreshToken is finally used
         */
        public async Task<AuthResponse> Handle(AuthCommand command, CancellationToken cancellationToken)
        {
            var customer = await _userService.CheckVerifiedUserAsync(command);
            
            var token = _tokenService.GenerateToken(new Dictionary<string, string>()
            {
                { "id", customer.Id.ToString() },
                { "email", customer.Email },
                { "phone", customer.Phone },
                { "firstName", customer.FirstName },
                { "lastName", customer.LastName },
                { "documentNumber", customer.DocumentNumber }
            });
            
            var refreshToken = "";
            
            var lastSession = await _sessionService.GetLastUserSessionAsync(null, customer.Id);

            if (lastSession == null)
            {
                refreshToken = _tokenService.GenerateRefreshToken();
                
                await _sessionService.InsertNewSessionAsync(customer, refreshToken);
            }
            else
            {
                if ( _sessionService.CheckIfRefreshTokenExpired(lastSession) )
                {
                    refreshToken = _tokenService.GenerateRefreshToken();
                
                    await _sessionService.InsertNewSessionAsync(customer, refreshToken);

                    lastSession.Used = true;
                
                    await _sessionService.UpdateSessionAsync(lastSession);
                }
                else
                {
                    refreshToken = lastSession.RefreshToken;
                }
            }

            return new AuthResponse()
            {
                Token = token,
                RefreshToken = refreshToken,
                Lifetime = _tokenConfiguration.Expiration,
                Expiration = DateTime.UtcNow.AddSeconds(_tokenConfiguration.Expiration),
                MD5 = _tokenService.GetMD5(token),
                UserId = customer.Id
            };
        }
    } 
};
