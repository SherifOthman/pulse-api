using MediatR;

namespace Pulse.API.Features.Auth.Login;

public class LoginEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/auth/login", async (LoginCommand command, HttpContext context, IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            try
            {
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
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });

        app.MapPost("/mobile/auth/login", async (LoginCommand command, HttpContext context, IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            try
            {
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
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });
    }
}
