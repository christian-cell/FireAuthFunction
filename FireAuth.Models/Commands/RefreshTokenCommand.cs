using FireAuth.Models.Responses;
using MediatR;

namespace FireAuth.Models.Commands
{
    public class RefreshTokenCommand : IRequest<AuthResponse>
    {
        public string RefreshToken { get; set; }
    }
};

