using Pulse.API.Features.Auth.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Pulse.API.Features.Auth.RefreshTokenFeature;

public class RefreshTokenEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/auth/refresh", async (
            HttpContext context,
            IMediator mediator,
            IWebHostEnvironment env) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (!context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Results.Unauthorized();

            try
            {
                var result = await mediator.Send(new RefreshTokenCommand(
                    string.Empty, refreshToken!, ip));

                context.Response.Cookies.Append("refreshToken", result.RefreshToken, BuildCookieOptions(env));

                return Results.Ok(new { result.AccessToken, result.RefreshToken });
            }
            catch
            {
                return Results.Unauthorized();
            }
        });

        app.MapPost("/mobile/auth/refresh", async (
            HttpContext context,
            IMediator mediator,
            IWebHostEnvironment env) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (!context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Results.Unauthorized();

            try
            {
                var result = await mediator.Send(new RefreshTokenCommand(
                    string.Empty, refreshToken!, ip));

                context.Response.Cookies.Append("refreshToken", result.RefreshToken, BuildCookieOptions(env));

                return Results.Ok(new { result.AccessToken, result.RefreshToken });
            }
            catch
            {
                return Results.Unauthorized();
            }
        });
    }

    private static CookieOptions BuildCookieOptions(IWebHostEnvironment env)
    {
        var isProd = env.IsProduction();
        return new CookieOptions
        {
            HttpOnly = true,
            SameSite = isProd ? SameSiteMode.None : SameSiteMode.Lax,
            Secure   = isProd,
            Path     = "/",
            Expires  = DateTime.UtcNow.AddDays(7)
        };
    }
}
