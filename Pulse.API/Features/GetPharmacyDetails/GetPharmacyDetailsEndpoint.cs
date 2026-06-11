using Pulse.API.Features;
using MediatR;

namespace Pulse.API.Features.Pharmacies.GetPharmacyDetails;

public class GetPharmacyDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/pharmacies/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetPharmacyDetailsQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
