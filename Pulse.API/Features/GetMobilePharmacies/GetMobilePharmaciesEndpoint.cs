using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Pharmacies.GetMobilePharmacies;

public class GetMobilePharmaciesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/pharmacies", async (HttpContext httpContext, IMediator mediator,
            Guid? governorateId, Guid? cityId, string? name,
            string? sortBy, string? sortDirection,
            int page = 1, int pageSize = 10) =>
        {
            var bq = new BusinessQuery(governorateId, cityId, name, sortBy, sortDirection, page, pageSize);
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var result = await mediator.Send(new GetMobilePharmaciesQuery(bq, baseUrl));
            return Results.Ok(result);
        });
    }
}
