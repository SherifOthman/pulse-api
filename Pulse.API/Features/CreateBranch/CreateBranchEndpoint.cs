using MediatR;

namespace Pulse.API.Features.Doctors.Branches.CreateBranch;

public class CreateBranchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/doctors/{doctorId:guid}/branches", async (Guid doctorId, CreateBranchCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { DoctorId = doctorId });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
