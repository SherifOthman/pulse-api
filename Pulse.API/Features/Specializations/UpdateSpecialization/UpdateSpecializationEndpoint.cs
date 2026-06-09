using MediatR;

namespace Pulse.API.Features.Specializations.UpdateSpecialization;

public class UpdateSpecializationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/specializations/{id:guid}", async (Guid id, UpdateSpecializationCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
