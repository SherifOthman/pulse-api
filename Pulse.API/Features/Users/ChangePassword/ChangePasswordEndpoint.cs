using Pulse.API.Infrastructure;
using MediatR;

namespace Pulse.API.Features.Users.ChangePassword;

public class ChangePasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/users/me/change-password", async (ChangePasswordRequest request, IMediator mediator, ICurrentUser currentUser) =>
        {
            try
            {
                var result = await mediator.Send(new ChangePasswordCommand(
                    request.CurrentPassword,
                    request.NewPassword,
                    currentUser.Id
                ));
                return Results.Ok(result);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
