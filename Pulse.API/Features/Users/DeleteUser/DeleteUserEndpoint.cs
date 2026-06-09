using MediatR;

namespace Pulse.API.Features.Users.DeleteUser;

public class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/admin/users/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteUserCommand(id));
            return Results.NoContent();
        }).RequireAuthorization("AdminOnly");
    }
}
