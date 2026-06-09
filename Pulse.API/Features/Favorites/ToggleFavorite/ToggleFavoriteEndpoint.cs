using Pulse.API.Features;
using MediatR;

namespace Pulse.API.Features.Favorites.ToggleFavorite;

public class ToggleFavoriteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/favorites/{businessId:guid}/toggle", async (Guid businessId, IMediator mediator) =>
        {
            await mediator.Send(new ToggleFavoriteCommand(businessId));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
