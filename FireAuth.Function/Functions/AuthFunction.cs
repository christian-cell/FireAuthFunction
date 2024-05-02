using System.Net;
using System.Text.Json;
using FireAuth.Domain.Entities;
using FireAuth.Domain.Infraestructure;
using FireAuth.Models.Commands;
using FireAuth.Models.Dtos;
using FireAuth.Models.Responses;
using FireAuth.Repository;
using FireAuth.Services.Abstractions.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;

namespace FireAuth.Function.Functions
{
    public class AuthFunction : EntityRepository
    {
        
        private readonly ILogger<AuthFunction> _logger;
        private readonly IUserService _userService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IMediator _mediator;
        
        public AuthFunction(
            IMediator mediator,
            IUserService userService,
            ICryptographyService cryptographyService,
            ILoggerFactory logger,
            UserDbContext dbContext
        ):base(dbContext)
        {
            _mediator = mediator;
            _userService = userService;
            _cryptographyService = cryptographyService;
            _logger = logger.CreateLogger<AuthFunction>();
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="req"> The HTTP request data.</param>
        /// <returns> The response containing user created data </return>
        [Function("create-user")]
        [AllowAnonymous]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "user" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserDto), Required = true, Description = "the minimum customer data to create new user")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Task<ActionResult>), Description = "The response")]
        public async Task<ActionResult> RegisterAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                StreamReader reader = new StreamReader(req.Body);

                UserDto userCommand = JsonSerializer.Deserialize<UserDto>(reader.ReadToEnd(), options) ?? new UserDto();

                if (userCommand == null) throw new Exception("user body can not be null");

                _logger.LogInformation($"... lets create user {userCommand.FirstName}");

                _userService.CheckPasswordsMatch(userCommand.Password, userCommand.PasswordConfirm);

                await _userService.CheckUserExistsAsync(userCommand.Email);
                
                string salt = _cryptographyService.GenerateSalt();

                string passwordHash = _cryptographyService.CreateHash(userCommand.Password, salt);

                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    FirstName = userCommand.FirstName,
                    LastName = userCommand.LastName,
                    DocumentNumber = userCommand.DocumentNumber,
                    Email = userCommand.Email,
                    Phone = userCommand.Phone,
                    Salt = salt,
                    PasswordHash = passwordHash,
                    Active = userCommand.Active
                };

                var result = _userService.CreateUser(user);
                
                return new OkObjectResult(new { result = result });
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"something went wrong creating new user {ex}");
                
                throw new Exception($"something went wrong creating new user {ex}");
            }
            
        }
        
        // <summary>
        /// Create new user
        /// </summary>
        /// <param name="req"> The HTTP request data.</param>
        /// <returns> The response containing user created data </return>
        [Function("auth-user")]
        [AllowAnonymous]
        [OpenApiOperation(operationId: "AuthUser", tags: new[] { "user" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AuthCommand), Required = true, Description = "the command to authenticate user")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Task<ActionResult<AuthResponse>>), Description = "The response")]
        public async Task<ActionResult> AuthAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                StreamReader reader = new StreamReader(req.Body);

                if (options == null) throw new Exception($"reader can not be null"); 

                AuthCommand authCommand = JsonSerializer.Deserialize<AuthCommand>(reader.ReadToEnd(), options) ?? new AuthCommand();

                _logger.LogInformation($"... let's auth user with email {authCommand.Email}");
                
                var response = await _mediator.Send(authCommand);

                return new OkObjectResult(new { result = response });
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"something went wrong authenticating user {ex}");
                
                throw new Exception($"something went wrong authenticating user {ex}");
            }
        }
        
        // <summary>
        /// Resfresh access token while refreshToken is not expired
        /// </summary>
        /// <param name="req"> The HTTP request data.</param>
        /// <returns> The response contains a new AuthResponse with the refreshed access token </return>
        [Function("refreshToken")]
        [AllowAnonymous]
        [OpenApiOperation(operationId: "RefreshToken", tags: new[] { "userToken" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(RefreshTokenCommand), Required = true, Description = "the refresh token we need to refresh access token")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Task<ActionResult<AuthResponse>>), Description = "The response")]
        public async Task<ActionResult> RefreshTokenAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                StreamReader reader = new StreamReader(req.Body);

                if (options == null) throw new Exception($"reader can not be null"); 

                RefreshTokenCommand refreshTokenCommand = JsonSerializer.Deserialize<RefreshTokenCommand>(reader.ReadToEnd(), options) ?? new RefreshTokenCommand();
                
                _logger.LogInformation($"... let's refresh token {refreshTokenCommand.RefreshToken}");

                var response = await _mediator.Send(refreshTokenCommand);

                return new OkObjectResult(new { result = response });
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"something went wrong refreshing user access token {ex}");
                
                throw new Exception($"something went wrong refreshing user access token {ex}");
            }
        }
    } 
};

