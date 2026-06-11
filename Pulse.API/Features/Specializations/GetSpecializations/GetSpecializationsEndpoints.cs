using MediatR;

namespace Pulse.API.Features.Specializations.GetSpecializations;

public class GetSpecializationsEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/specializations", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSpecializationsQuery());
            return Results.Ok(result);
        });

        app.MapGet("/mobile/specializations", async (IMediator mediator, int? businessType) =>
        {
            var result = await mediator.Send(new GetSpecializationsQuery(businessType));
            return Results.Ok(result);
        });
    }
}
