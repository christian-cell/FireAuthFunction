using FireAuth.Domain.Entities;
using FireAuth.Models.Commands;

namespace FireAuth.Services.Abstractions.Auth
{
    public interface IUserService
    {
        Task CheckUserExistsAsync(string email);

        Task<User> CreateUser(User customer);

        string GetUser();
        Task<User> CheckVerifiedUserAsync(AuthCommand command);
        void CheckPasswordsMatch(string Password, string PasswordConfirm);
    }
};

