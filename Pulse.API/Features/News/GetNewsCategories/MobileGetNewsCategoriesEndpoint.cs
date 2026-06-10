using MediatR;

namespace Pulse.API.Features.News.GetNewsCategories;

public class MobileGetNewsCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/news/categories", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetNewsCategoriesQuery());
            return Results.Ok(result);
        });
    }
}
