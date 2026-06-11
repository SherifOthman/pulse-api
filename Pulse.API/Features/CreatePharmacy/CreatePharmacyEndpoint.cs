using MediatR;

namespace Pulse.API.Features.Pharmacies.CreatePharmacy;

public class CreatePharmacyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/pharmacies", async (CreatePharmacyCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/dashboard/pharmacies/{result.Id}", result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
