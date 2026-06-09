using Pulse.API.Features.Favorites.GetFavorites;
using MediatR;

namespace Pulse.API.Features.Favorites.GetFavorites;

public class MobileGetFavoritesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/favorites", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetFavoritesQuery());
            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
