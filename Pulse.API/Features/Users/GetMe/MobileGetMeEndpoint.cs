using Pulse.API.Features.Users.GetMe;
using Pulse.API.Infrastructure;
using MediatR;

namespace Pulse.API.Features.Users.GetMe;

public class MobileGetMeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
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
