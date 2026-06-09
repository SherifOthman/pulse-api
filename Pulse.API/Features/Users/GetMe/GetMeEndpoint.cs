using Pulse.API.Infrastructure;
using MediatR;

namespace Pulse.API.Features.Users.GetMe;

public class GetMeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me", async (IMediator mediator, ICurrentUser currentUser) =>
        {
            var result = await mediator.Send(new GetMeQuery(currentUser.Id.ToString()));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
