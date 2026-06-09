using MediatR;

namespace Pulse.API.Features.Specializations.CreateSpecialization;

public class CreateSpecializationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/specializations", async (CreateSpecializationCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
