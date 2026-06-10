using MediatR;

namespace Pulse.API.Features.News.GetNewsById;

public class MobileGetNewsByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/news/{id}", async (string id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetNewsByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
