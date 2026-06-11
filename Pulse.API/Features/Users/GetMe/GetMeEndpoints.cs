using Pulse.API.Infrastructure;
using MediatR;

namespace Pulse.API.Features.Users.GetMe;

public class GetMeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/users/me", async (IMediator mediator, ICurrentUser currentUser) =>
        {
            var result = await mediator.Send(new GetMeQuery(currentUser.Id.ToString()));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");

        app.MapGet("/mobile/users/me", async (IMediator mediator, ICurrentUser currentUser) =>
        {
            var result = await mediator.Send(new GetMeQuery(currentUser.Id.ToString()));
            return Results.Ok(new
            {
                result.Id,
                result.Email,
                result.FullName,
                result.ImageUrl,
                result.EmailConfirmed
            });
        }).RequireAuthorization();
    }
}
