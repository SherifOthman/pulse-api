using Pulse.API.Features.Favorites.ToggleFavorite;
using MediatR;

namespace Pulse.API.Features.Favorites.ToggleFavorite;

public class MobileToggleFavoriteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/mobile/favorites/{businessId:guid}/toggle", async (Guid businessId, IMediator mediator) =>
        {
            await mediator.Send(new ToggleFavoriteCommand(businessId));
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
