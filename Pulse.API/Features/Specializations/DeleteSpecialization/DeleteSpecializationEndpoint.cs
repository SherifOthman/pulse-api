using MediatR;

namespace Pulse.API.Features.Specializations.DeleteSpecialization;

public class DeleteSpecializationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/specializations/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteSpecializationCommand(id));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
