using Pulse.API.Infrastructure;
using Pulse.API.Infrastructure.Google;
using Pulse.API.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Pulse.API.DependencyInjections;

public static class JWTAuthDep
{
    public static void AddJWTAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection("Jwt").Get<JWTOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = jwtOptions!.Issuer,
                    ValidAudience = jwtOptions!.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Admin", "Manager"));
        });

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
        services.Configure<JWTOptions>(configuration.GetSection("Jwt"));
        services.Configure<GoogleAuthOptions>(configuration.GetSection("GoogleAuth"));
    }
}

