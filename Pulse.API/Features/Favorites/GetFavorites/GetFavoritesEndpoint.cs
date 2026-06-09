using MediatR;

namespace Pulse.API.Features.Favorites.GetFavorites;

public class GetFavoritesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/favorites", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetFavoritesQuery());
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
