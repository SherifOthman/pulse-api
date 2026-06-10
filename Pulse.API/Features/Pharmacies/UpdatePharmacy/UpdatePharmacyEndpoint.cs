using MediatR;

namespace Pulse.API.Features.Pharmacies.UpdatePharmacy;

public class UpdatePharmacyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/pharmacies/{id:guid}", async (Guid id, UpdatePharmacyCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
