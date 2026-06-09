using MediatR;

namespace Pulse.API.Features.Governorates.GetGovernorateCities;

public class GetGovernorateCitiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/governorates/{id:guid}/cities", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetGovernorateCitiesQuery(id));
            return Results.Ok(result);
        });
    }
}
