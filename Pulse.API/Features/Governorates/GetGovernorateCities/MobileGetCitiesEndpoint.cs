using Pulse.API.Features.Governorates.GetGovernorateCities;
using MediatR;

namespace Pulse.API.Features.Governorates.GetGovernorateCities;

public class MobileGetCitiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/governorates/{id:guid}/cities", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetGovernorateCitiesQuery(id));
            return Results.Ok(result);
        });
    }
}
