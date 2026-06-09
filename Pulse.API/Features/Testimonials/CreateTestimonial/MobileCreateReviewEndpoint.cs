using Pulse.API.Features.Testimonials.CreateTestimonial;
using MediatR;

namespace Pulse.API.Features.Testimonials.CreateTestimonial;

public class MobileCreateReviewEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/mobile/reviews", async (CreateTestimonialCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/mobile/reviews/{result.Id}", result);
        }).RequireAuthorization();
    }
}
