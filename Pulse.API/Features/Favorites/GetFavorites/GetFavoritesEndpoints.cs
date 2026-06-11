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

        app.MapGet("/mobile/favorites", async (HttpContext httpContext, IMediator mediator) =>
        {
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var result = await mediator.Send(new GetFavoritesQuery(baseUrl));
            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
