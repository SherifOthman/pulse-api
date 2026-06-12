using Pulse.API.Features.Auth.Login;
using Pulse.API.Features.Auth.RefreshToken;
using MediatR;

namespace Pulse.API.Features.Auth.RefreshTokenFeature;

public class RefreshTokenEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/auth/refresh", async (
            HttpContext context,
            IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (!context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Results.Unauthorized();

            try
            {
                var result = await mediator.Send(new RefreshTokenCommand(
                    string.Empty, refreshToken!, ip));

                LoginEndpoints.AppendRefreshTokenCookie(context, result.RefreshToken);
                return Results.Ok(new { result.AccessToken, result.RefreshToken });
            }
            catch
            {
                return Results.Unauthorized();
            }
        });

        app.MapPost("/mobile/auth/refresh", async (
            HttpContext context,
            IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (!context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Results.Unauthorized();

            try
            {
                var result = await mediator.Send(new RefreshTokenCommand(
                    string.Empty, refreshToken!, ip));

                LoginEndpoints.AppendRefreshTokenCookie(context, result.RefreshToken);
                return Results.Ok(new { result.AccessToken, result.RefreshToken });
            }
            catch
            {
                return Results.Unauthorized();
            }
        });
    }
}
