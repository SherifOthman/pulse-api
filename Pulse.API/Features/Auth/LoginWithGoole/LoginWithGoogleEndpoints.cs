using MediatR;
using Pulse.API.Features.Auth.Login;

namespace Pulse.API.Features.Auth.LoginWithGoole;

public class LoginWithGoogleEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/auth/google", async (
            LoginWithGoogleCommand command,
            HttpContext context,
            IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            var result = await mediator.Send(command with { IpAddress = ip });
            LoginEndpoints.AppendRefreshTokenCookie(context, result.RefreshToken);
            return Results.Ok(new { result.AccessToken, result.RefreshToken });
        });

        app.MapPost("/mobile/auth/google", async (
            LoginWithGoogleCommand command,
            HttpContext context,
            IMediator mediator) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            var result = await mediator.Send(command with { IpAddress = ip });
            LoginEndpoints.AppendRefreshTokenCookie(context, result.RefreshToken);
            return Results.Ok(new { result.AccessToken, result.RefreshToken });
        });
    }
}
