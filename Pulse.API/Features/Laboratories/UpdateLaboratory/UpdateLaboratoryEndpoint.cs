using MediatR;

namespace Pulse.API.Features.Laboratories.UpdateLaboratory;

public class UpdateLaboratoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/labs/{id:guid}", async (Guid id, UpdateLaboratoryCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
