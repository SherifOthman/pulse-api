using MediatR;

namespace Pulse.API.Features.Favorites.GetFavorites;

public class GetFavoritesEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/favorites", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetFavoritesQuery());
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");

        app.MapGet("/mobile/favorites", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetFavoritesQuery());
            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
