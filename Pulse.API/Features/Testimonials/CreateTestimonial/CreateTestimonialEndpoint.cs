using MediatR;

namespace Pulse.API.Features.Testimonials.CreateTestimonial;

public class CreateTestimonialEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/reviews", async (CreateTestimonialCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/reviews/{result.Id}", result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
