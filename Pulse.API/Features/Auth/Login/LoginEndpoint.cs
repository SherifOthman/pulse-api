using Pulse.API.Features;
using MediatR;

namespace Pulse.API.Features.Auth.Login;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (LoginCommand command, HttpContext context, IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            try
            {
                var result = await mediator.Send(command with { IpAddress = ip });

                context.Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Results.Ok(new { result.AccessToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });
    }
}
