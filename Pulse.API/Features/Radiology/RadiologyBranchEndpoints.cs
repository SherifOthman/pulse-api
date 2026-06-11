using MediatR;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared.Branches.CreateBranch;
using Pulse.API.Features.Shared.Branches.DeleteBranch;
using Pulse.API.Features.Shared.Branches.GetBranchDetails;
using Pulse.API.Features.Shared.Branches.GetBranches;
using Pulse.API.Features.Shared.Branches.UpdateBranch;

namespace Pulse.API.Features.Radiology;

public class RadiologyBranchEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var type = BusinessType.Radiology;
        var prefix = "/dashboard/radiology/{businessId:guid}/branches";

        app.MapGet(prefix, async (Guid businessId, IMediator mediator) =>
        {
            var result = await mediator.Send(new SharedGetBranchesQuery(businessId, type));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");

        app.MapGet($"{prefix}/{{id:guid}}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new SharedGetBranchDetailsQuery(id, type));
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");

        app.MapPost(prefix, async (Guid businessId, SharedCreateBranchCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { BusinessId = businessId, Type = type });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");

        app.MapPut($"{prefix}/{{id:guid}}", async (Guid id, SharedUpdateBranchCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id, Type = type });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");

        app.MapDelete($"{prefix}/{{id:guid}}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new SharedDeleteBranchCommand(id, type));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
