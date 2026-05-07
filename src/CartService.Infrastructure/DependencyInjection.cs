using CartService.Application.Common.Interfaces;
using CartService.Infrastructure.Cache;
using CartService.Infrastructure.Events;
using CartService.Infrastructure.Health;
using CartService.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;

namespace CartService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Settings
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));

        var jwt = configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        // JWT bearer authentication
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                    ValidateLifetime = true
                };
            });

        // Redis
        var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value
            ?? throw new InvalidOperationException("Redis:ConnectionString is missing.");

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var s = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
            return new ConnectionFactory
            {
                HostName = s.Host,
                Port = s.Port,
                UserName = s.Username,
                Password = s.Password,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(3)
            };
        });

        services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("redis")
            .AddCheck<RabbitMqHealthCheck>("rabbitmq");

        // Infrastructure services
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddSingleton<IEventPublisher, RabbitMqPublisher>();

        return services;
    }
}
