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
        // Always use SameSite=None + Secure=true.
        // The dashboard (Vercel) and API (runasp.net) are on different domains,
        // so cross-origin cookie rules apply. SameSite=None requires Secure=true.
        // The API is always served over HTTPS in production (runasp.net terminates TLS),
        // so Secure=true is always safe. In local dev the cookie works regardless
        // because dashboard and API share the same origin (localhost).
        context.Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure   = true,
            Path     = "/",
            Expires  = DateTime.UtcNow.AddDays(7)
        });
    }
}
