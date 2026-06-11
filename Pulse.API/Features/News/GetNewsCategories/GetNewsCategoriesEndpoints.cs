using MediatR;

namespace Pulse.API.Features.News.GetNewsCategories;

public class GetNewsCategoriesEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/news/categories", async (IMediator mediator) =>
        {
            var categories = await mediator.Send(new GetNewsCategoriesQuery());
            return Results.Ok(categories);
        });

        app.MapGet("/mobile/news/categories", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetNewsCategoriesQuery());
            return Results.Ok(result);
        });
    }
}
