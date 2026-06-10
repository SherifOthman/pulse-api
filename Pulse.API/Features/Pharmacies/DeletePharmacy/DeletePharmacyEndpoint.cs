using MediatR;

namespace Pulse.API.Features.Pharmacies.DeletePharmacy;

public class DeletePharmacyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/dashboard/pharmacies/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeletePharmacyCommand(id));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
