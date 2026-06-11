using MediatR;

namespace Pulse.API.Features.Doctors.Services.UpdateDoctorServices;

public class UpdateDoctorServicesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // PUT replaces the full service list for a doctor
        app.MapPut("/dashboard/doctors/{doctorId:guid}/services",
            async (Guid doctorId, UpdateDoctorServicesCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command with { DoctorId = doctorId });
                return Results.Ok(result);
            })
            .RequireAuthorization("ManagerOrAdmin");
    }
}
