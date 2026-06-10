using MediatR;

namespace Pulse.API.Features.Doctors.Branches.GetBranches;

public class GetBranchesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/doctors/{doctorId:guid}/branches", async (Guid doctorId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetBranchesQuery(doctorId));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
