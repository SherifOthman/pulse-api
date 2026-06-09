using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Radiology.GetMobileRadiology;

public class GetMobileRadiologyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/radiology", async (IMediator mediator,
            Guid? governorateId,
            Guid? cityId,
            string? name,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 10) =>
        {
            var bq = new BusinessQuery(governorateId, cityId, name, sortBy, sortDirection, page, pageSize);
            var result = await mediator.Send(new GetMobileRadiologyQuery(bq));
            return Results.Ok(result);
        });
    }
}
