using MediatR;

namespace Pulse.API.Features.Doctors.UpdateDoctor;

public class UpdateDoctorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/doctors/{id:guid}", async (Guid id, UpdateDoctorCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
