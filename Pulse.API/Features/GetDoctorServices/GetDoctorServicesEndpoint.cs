using MediatR;

namespace Pulse.API.Features.Doctors.Services.GetDoctorServices;

public class GetDoctorServicesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/doctors/{doctorId:guid}/services", async (Guid doctorId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetDoctorServicesQuery(doctorId));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
