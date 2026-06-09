using Pulse.API.Features.Specializations.GetSpecializations;
using MediatR;

namespace Pulse.API.Features.Specializations.GetSpecializations;

public class MobileGetSpecializationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/specializations", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSpecializationsQuery());
            return Results.Ok(result);
        });
    }
}
