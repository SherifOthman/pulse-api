using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Laboratories.GetLaboratories;

public class GetLaboratoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/labs", async (IMediator mediator,
            Guid? governorateId,
            Guid? cityId,
            string? name,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 10) =>
        {
            var bq = new BusinessQuery(governorateId, cityId, name, sortBy, sortDirection, page, pageSize);
            var result = await mediator.Send(new GetLaboratoriesQuery(bq));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
