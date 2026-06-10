using MediatR;

namespace Pulse.API.Features.Radiology.DeleteRadiology;

public class DeleteRadiologyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/dashboard/radiology/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteRadiologyCommand(id));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
