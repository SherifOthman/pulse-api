using MediatR;

namespace Pulse.API.Features.Auth.Login;

public class LoginEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/auth/login", async (
            LoginCommand command,
            HttpContext context,
            IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            try
            {
                var result = await mediator.Send(command with { IpAddress = ip });
                AppendRefreshTokenCookie(context, result.RefreshToken);
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
            IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            try
            {
                var result = await mediator.Send(command with { IpAddress = ip });
                AppendRefreshTokenCookie(context, result.RefreshToken);
                return Results.Ok(new { result.AccessToken, result.RefreshToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });
    }

    internal static void AppendRefreshTokenCookie(HttpContext context, string token)
    {
        // SameSite=None + Secure is required for cross-origin cookie access
        // (dashboard on Vercel <-> API on runasp.net).
        // With UseForwardedHeaders in place, context.Request.IsHttps reflects
        // the actual scheme the client used, so Secure is set correctly in both
        // prod (HTTPS) and local dev (HTTP — cookie still works same-origin).
        var isHttps = context.Request.IsHttps;

        context.Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Secure   = isHttps,
            Path     = "/",
            Expires  = DateTime.UtcNow.AddDays(7)
        });
    }
}
