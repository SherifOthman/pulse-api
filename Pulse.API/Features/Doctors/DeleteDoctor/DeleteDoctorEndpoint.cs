using MediatR;

namespace Pulse.API.Features.Doctors.DeleteDoctor;

public class DeleteDoctorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/dashboard/doctors/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteDoctorCommand(id));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
