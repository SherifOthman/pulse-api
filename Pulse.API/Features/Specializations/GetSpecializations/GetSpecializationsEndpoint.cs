using MediatR;

namespace Pulse.API.Features.Specializations.GetSpecializations;

public class GetSpecializationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/specializations", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSpecializationsQuery());
            return Results.Ok(result);
        });
    }
}
