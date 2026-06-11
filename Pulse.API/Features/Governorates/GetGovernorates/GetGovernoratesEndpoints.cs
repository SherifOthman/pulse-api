using MediatR;

namespace Pulse.API.Features.Governorates.GetGovernorates;

public class GetGovernoratesEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/governorates", async (IMediator mediator, int? businessType) =>
        {
            var result = await mediator.Send(new GetGovernoratesQuery(businessType));
            return Results.Ok(result);
        });

        app.MapGet("/mobile/governorates", async (IMediator mediator, int? businessType) =>
        {
            var result = await mediator.Send(new GetGovernoratesQuery(businessType));
            return Results.Ok(result);
        });
    }
}
