using FireAuth.Models.Responses;
using MediatR;

namespace FireAuth.Models.Commands
{
    public class AuthCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
};

