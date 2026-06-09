using MediatR;

namespace Pulse.API.Features.Pharmacies.CreatePharmacy;

public class CreatePharmacyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/pharmacies", async (CreatePharmacyCommand command, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
