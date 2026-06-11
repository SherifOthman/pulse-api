using MediatR;

namespace Pulse.API.Features.Doctors.Branches.UpdateBranch;

public class UpdateBranchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/doctors/{doctorId:guid}/branches/{id:guid}", async (Guid id, UpdateBranchCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
