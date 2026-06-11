using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Pharmacies.GetPharmacies;

public class GetPharmaciesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/pharmacies", async (IMediator mediator,
            Guid? governorateId,
            Guid? cityId,
            string? name,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 10) =>
        {
            var bq = new BusinessQuery(governorateId, cityId, name, sortBy, sortDirection, page, pageSize);
            var result = await mediator.Send(new GetPharmaciesQuery(bq));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
