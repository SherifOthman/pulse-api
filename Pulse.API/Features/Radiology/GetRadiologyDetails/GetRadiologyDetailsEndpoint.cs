using Pulse.API.Features;
using MediatR;

namespace Pulse.API.Features.Radiology.GetRadiologyDetails;

public class GetRadiologyDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/radiology/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetRadiologyDetailsQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
