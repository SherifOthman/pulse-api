using Pulse.API.Features;
using MediatR;

namespace Pulse.API.Features.Laboratories.GetLaboratoryDetails;

public class GetLaboratoryDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/labs/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetLaboratoryDetailsQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
