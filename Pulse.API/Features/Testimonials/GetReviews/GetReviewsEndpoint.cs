using MediatR;

namespace Pulse.API.Features.Testimonials.GetReviews;

public class GetReviewsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/businesses/{id:guid}/reviews", async (
            Guid id,
            IMediator mediator,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default) =>
        {
            var result = await mediator.Send(new GetReviewsQuery(id, page, pageSize), ct);
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
