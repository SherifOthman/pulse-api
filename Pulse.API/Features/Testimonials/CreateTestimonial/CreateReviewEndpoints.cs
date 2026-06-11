using MediatR;

namespace Pulse.API.Features.Testimonials.CreateTestimonial;

public class CreateReviewEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/reviews", async (CreateTestimonialCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/reviews/{result.Id}", result);
        }).RequireAuthorization("ManagerOrAdmin");

        app.MapPost("/mobile/reviews", async (CreateTestimonialCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/mobile/reviews/{result.Id}", result);
        }).RequireAuthorization();
    }
}
