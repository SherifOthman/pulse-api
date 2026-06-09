using Pulse.API.Features.Auth.LoginWithGoole;
using MediatR;

namespace Pulse.API.Features.Auth.LoginWithGoole;

public class MobileGoogleLoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/mobile/auth/google", async (LoginWithGoogleCommand command, HttpContext context, IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            var result = await mediator.Send(command with { IpAddress = ip });

            context.Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Results.Ok(new { result.AccessToken, result.RefreshToken });
        });
    }
}
