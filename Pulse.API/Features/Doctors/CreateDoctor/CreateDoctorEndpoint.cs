using MediatR;

namespace Pulse.API.Features.Doctors.CreateDoctor;

public class CreateDoctorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/doctors", async (CreateDoctorCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/dashboard/doctors/{result.Id}", result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
