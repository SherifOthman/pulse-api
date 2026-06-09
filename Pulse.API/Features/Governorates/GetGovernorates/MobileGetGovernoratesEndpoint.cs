using Pulse.API.Features.Governorates.GetGovernorates;
using MediatR;

namespace Pulse.API.Features.Governorates.GetGovernorates;

public class MobileGetGovernoratesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/governorates", async (IMediator mediator, int? businessType) =>
        {
            var result = await mediator.Send(new GetGovernoratesQuery(businessType));
            return Results.Ok(result);
        });
    }
}
