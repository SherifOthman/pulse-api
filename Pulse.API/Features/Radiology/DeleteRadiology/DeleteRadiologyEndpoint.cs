using MediatR;

namespace Pulse.API.Features.Radiology.DeleteRadiology;

public class DeleteRadiologyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/radiology/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            try
            {
                await mediator.Send(new DeleteRadiologyCommand(id));
                return Results.NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
