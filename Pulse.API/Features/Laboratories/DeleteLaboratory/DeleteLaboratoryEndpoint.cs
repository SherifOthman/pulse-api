using MediatR;

namespace Pulse.API.Features.Laboratories.DeleteLaboratory;

public class DeleteLaboratoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/labs/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteLaboratoryCommand(id));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
