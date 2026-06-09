using MediatR;

namespace Pulse.API.Features.News.GetNewsById;

public class GetNewsByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/news/{id}", async (string id, IMediator mediator) =>
        {
            var article = await mediator.Send(new GetNewsByIdQuery(id));
            return article is not null ? Results.Ok(article) : Results.NotFound();
        });
    }
}
