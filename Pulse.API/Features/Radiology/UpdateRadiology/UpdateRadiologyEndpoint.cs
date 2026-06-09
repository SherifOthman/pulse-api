using MediatR;

namespace Pulse.API.Features.Radiology.UpdateRadiology;

public class UpdateRadiologyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/radiology/{id:guid}", async (Guid id, UpdateRadiologyCommand command, IMediator mediator) =>
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
