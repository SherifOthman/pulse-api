using MediatR;

namespace Pulse.API.Features.Doctors.CreateDoctor;

public class CreateDoctorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/doctors", async (CreateDoctorCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
