using System.Security;
using FireAuth.Domain.Entities;
using FireAuth.Domain.Infraestructure;
using FireAuth.Models.Commands;
using FireAuth.Models.Exceptions;
using FireAuth.Models.Models;
using FireAuth.Repository;
using FireAuth.Services.Abstractions.Auth;
using FireAuths.Services.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FireAuth.Services.Services.Auth
{
    public class UserService : EntityRepository , IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        private readonly ICryptographyService _cryptographyService;

        public UserService(
            ICryptographyService cryptographyService,
            IHttpContextAccessor httpContextAccessor,
            UserDbContext userDbContext,
            ILogger<UserService> logger
            )
            :base(userDbContext)
        {
            _cryptographyService = cryptographyService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CheckUserExistsAsync(string email)
        {
        
            var customer = await FirstOrDefaultAsync<User>(x =>
                x.Email == email
            );

            if (customer != null)throw new AlreadyExistsException("Customer" , "Email", email);
        }

        public async Task<User> CreateUser( User user )
        {
        
            var result = await AddAsync<User>(user);
         
            bool ok = await SaveAsync().ConfigureAwait(false);
         
            if (result is null || !ok)
            {
                _logger.LogError("Something went wrong creating a new user");
 
                throw new Exception("Something went wrong creating a new User");
            }
 
            return user;
        }
        
        public async Task<User> CheckVerifiedUserAsync(AuthCommand command)
        {
            var user = await FirstOrDefaultAsync<User>(x =>
                x.Email == command.Email
            );

            if (user is null) throw new Exception($"The user with email {command.Email} not found");
            
            if (!user.Active) throw new SecurityException($"The user {command.Email} has not been activated");
            
            var verified = _cryptographyService.ValidatePasswordAndHash(command.Password, user.Salt, user.PasswordHash);

            if (!verified) throw new SecurityException($"Not valid credentials");

            return user;
        }

        public string GetUser()
        {

            return ClaimExtension.GetValueFromUserClaims<string>(_httpContextAccessor.HttpContext, UserClaim.Id);
        }
        
        public void CheckPasswordsMatch(string password, string passwordConfirm)
        {
            if (password != passwordConfirm) throw new Exception($"passwords don't match");
        }
    }
};

