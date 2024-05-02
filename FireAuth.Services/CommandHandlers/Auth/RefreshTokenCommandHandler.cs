using FireAuth.Models.Commands;
using FireAuth.Models.Responses;
using FireAuth.Services.Abstractions.Auth;
using MediatR;

namespace FireAuth.Services.CommandHandlers.Auth
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            return await _tokenService.RefreshTokenAsync(command.RefreshToken);
        }
    }
};