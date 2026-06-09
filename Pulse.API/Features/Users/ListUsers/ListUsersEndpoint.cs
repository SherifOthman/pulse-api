using MediatR;

namespace Pulse.API.Features.Users.ListUsers;

public class ListUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/admin/users", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new ListUsersQuery());
            return Results.Ok(result);
        }).RequireAuthorization("AdminOnly");
    }
}
