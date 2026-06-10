using MediatR;

namespace Pulse.API.Features.Doctors.Branches.GetBranchDetails;

public class GetBranchDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/doctors/{doctorId:guid}/branches/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetBranchDetailsQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
