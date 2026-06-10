using Pulse.API.Services;
using MediatR;

namespace Pulse.API.Features.News.GetNews;

public class MobileGetNewsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/news", async (IMediator mediator,
            int? page, int? pageSize, string? category, string? search) =>
        {
            var result = await mediator.Send(new GetNewsQuery(
                page ?? 1, pageSize ?? 10, category, search));
            return Results.Ok(result);
        });
    }
}
