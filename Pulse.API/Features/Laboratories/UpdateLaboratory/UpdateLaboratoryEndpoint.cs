using MediatR;

namespace Pulse.API.Features.Laboratories.UpdateLaboratory;

public class UpdateLaboratoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/labs/{id:guid}", async (Guid id, UpdateLaboratoryCommand command, IMediator mediator) =>
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
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
