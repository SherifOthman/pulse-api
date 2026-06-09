using MediatR;

namespace Pulse.API.Features.Pharmacies.DeletePharmacy;

public class DeletePharmacyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/pharmacies/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            try
            {
                await mediator.Send(new DeletePharmacyCommand(id));
                return Results.NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
