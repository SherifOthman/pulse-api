using MediatR;

namespace Pulse.API.Features.Governorates.GetGovernorates;

public class GetGovernoratesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/governorates", async (IMediator mediator, int? businessType) =>
        {
            var result = await mediator.Send(new GetGovernoratesQuery(businessType));
            return Results.Ok(result);
        });
    }
}
