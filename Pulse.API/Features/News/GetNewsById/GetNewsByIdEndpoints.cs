using MediatR;

namespace Pulse.API.Features.News.GetNewsById;

public class GetNewsByIdEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/news/{id}", async (string id, IMediator mediator) =>
        {
            var article = await mediator.Send(new GetNewsByIdQuery(id));
            return article is not null ? Results.Ok(article) : Results.NotFound();
        });

        app.MapGet("/mobile/news/{id}", async (string id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetNewsByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
