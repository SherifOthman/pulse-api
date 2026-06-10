using MediatR;

namespace Pulse.API.Features.Specializations.GetSpecializations;

public class MobileGetSpecializationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/specializations", async (IMediator mediator, int? businessType) =>
        {
            var result = await mediator.Send(new GetSpecializationsQuery(businessType));
            return Results.Ok(result);
        });
    }
}
