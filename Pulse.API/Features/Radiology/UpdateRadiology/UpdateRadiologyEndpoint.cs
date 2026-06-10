using MediatR;

namespace Pulse.API.Features.Radiology.UpdateRadiology;

public class UpdateRadiologyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/radiology/{id:guid}", async (Guid id, UpdateRadiologyCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
