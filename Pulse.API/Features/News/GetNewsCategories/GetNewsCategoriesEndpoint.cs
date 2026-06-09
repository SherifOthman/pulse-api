using MediatR;

namespace Pulse.API.Features.News.GetNewsCategories;

public class GetNewsCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/news/categories", async (IMediator mediator) =>
        {
            var categories = await mediator.Send(new GetNewsCategoriesQuery());
            return Results.Ok(categories);
        });
    }
}
