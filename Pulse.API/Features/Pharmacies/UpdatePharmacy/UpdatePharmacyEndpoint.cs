using MediatR;

namespace Pulse.API.Features.Pharmacies.UpdatePharmacy;

public class UpdatePharmacyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/pharmacies/{id:guid}", async (Guid id, UpdatePharmacyCommand command, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(command with { Id = id });
                return Results.Ok(result);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
