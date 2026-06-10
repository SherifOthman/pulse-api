using MediatR;

namespace Pulse.API.Features.Doctors.Services.GetAvailableServices;

public class GetAvailableServicesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Route must NOT look like /dashboard/doctors/{id:guid}/...
        // to avoid conflicts with the doctor detail endpoint.
        app.MapGet("/dashboard/doctor-services", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAvailableServicesQuery());
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
