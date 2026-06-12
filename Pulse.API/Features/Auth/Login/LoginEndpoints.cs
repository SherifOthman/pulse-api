using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Pulse.API.Features.Auth.Login;

public class LoginEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/auth/login", async (
            LoginCommand command,
            HttpContext context,
            IMediator mediator,
            IWebHostEnvironment env) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            try
            {
                var result = await mediator.Send(command with { IpAddress = ip });

                context.Response.Cookies.Append("refreshToken", result.RefreshToken, BuildCookieOptions(env));

                return Results.Ok(new { result.AccessToken, result.RefreshToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });

        app.MapPost("/mobile/auth/login", async (
            LoginCommand command,
            HttpContext context,
            IMediator mediator,
            IWebHostEnvironment env) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            try
            {
                var result = await mediator.Send(command with { IpAddress = ip });

                context.Response.Cookies.Append("refreshToken", result.RefreshToken, BuildCookieOptions(env));

                return Results.Ok(new { result.AccessToken, result.RefreshToken });
            }
            catch (UnauthorizedAccessException)
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
            // SameSite=None + Secure=true is required for cross-origin cookies (production).
            // In development the API and dashboard share localhost so Lax is fine over HTTP.
            SameSite = isProd ? SameSiteMode.None : SameSiteMode.Lax,
            Secure   = isProd,
            Path     = "/",
            Expires  = DateTime.UtcNow.AddDays(7)
        };
    }
}
