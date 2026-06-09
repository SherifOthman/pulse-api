using MediatR;

namespace Pulse.API.Features.Users.UpdateUser;

public class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/admin/users/{id:guid}", async (Guid id, UpdateUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id });
            return Results.Ok(result);
        }).RequireAuthorization("AdminOnly");
    }
}
