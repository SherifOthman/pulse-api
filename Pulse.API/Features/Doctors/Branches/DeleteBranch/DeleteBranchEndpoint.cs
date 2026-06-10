using MediatR;

namespace Pulse.API.Features.Doctors.Branches.DeleteBranch;

public class DeleteBranchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/dashboard/doctors/{doctorId:guid}/branches/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteBranchCommand(id));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
