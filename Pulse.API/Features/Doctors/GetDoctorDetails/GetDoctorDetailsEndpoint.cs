using Pulse.API.Features;
using MediatR;

namespace Pulse.API.Features.Doctors.GetDoctorDetails;

public class GetDoctorDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/doctors/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetDoctorDetailsQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
