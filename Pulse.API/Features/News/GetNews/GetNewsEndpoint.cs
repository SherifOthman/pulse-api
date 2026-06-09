using MediatR;

namespace Pulse.API.Features.News.GetNews;

public class GetNewsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/news", async (IMediator mediator,
            int? page, int? pageSize, string? category, string? search) =>
        {
            var result = await mediator.Send(new GetNewsQuery(
                page ?? 1, pageSize ?? 10, category, search));
            return Results.Ok(result);
        });
    }
}
