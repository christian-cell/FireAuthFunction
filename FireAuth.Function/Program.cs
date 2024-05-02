using FireAuth.Domain.Infraestructure;
using FireAuth.Models.Configurations;
using FireAuth.Services.Abstractions.Auth;
using FireAuth.Services.Extensions;
using FireAuth.Services.QueryHandlers;
using FireAuth.Services.Services.Auth;
using GoodDeal.API.Auth;
using GoodDeal.Services.CommandHandlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((appBuilder, services) =>
    {
        
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ICryptographyService, CryptographyService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<ISessionService, SessionService>();
        services.AddHttpContextAccessor();
        
        var tokenConfiguration = new TokenConfiguration()
        {
            Key = Environment.GetEnvironmentVariable("DonatorToken_Key") ?? string.Empty,
            Expiration = int.Parse(Environment.GetEnvironmentVariable("DonatorToken_Expiration")),
            RefreshTokenExpiration = int.Parse(Environment.GetEnvironmentVariable("DonatorToken_RefreshTokenExpiration")),
            Audience = Environment.GetEnvironmentVariable("DonatorToken_Audience") ?? string.Empty,
            Issuer = Environment.GetEnvironmentVariable("DonatorToken_Issuer")  ?? string.Empty
        };
        
        services.AddSingleton(tokenConfiguration);
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        
        /*Customer authorization*/
        services
            .AddAuthentication()
            .AddJwtBearer("CustomersJwtScheme", options => Jwt.ConfigureJwtService(options, tokenConfiguration));
        
        /*Mediatr*/
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<CommandHandlerDiscoveryType>();
            configuration.RegisterServicesFromAssemblyContaining<QueryHandlerDiscoveryType>();
        });
        
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection"));
        });
        
        services.Configure<OpenApiInfo>(options =>
        {
            options.Version = "v1";
            options.Title = "Azure Function API con Swagger";
            options.Description = "Descripción de la API";
            options.TermsOfService = new Uri("https://example.com/terms");
            options.Contact = new OpenApiContact
            {
                Name = "Contacto",
                Email = "contacto@example.com",
                Url = new Uri("https://example.com/contact"),
            };
            options.License = new OpenApiLicense
            {
                Name = "Licencia de Uso",
                Url = new Uri("https://example.com/license"),
            };
        });
    })
    .Build();

host.Run();