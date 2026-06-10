using MediatR;

namespace Pulse.API.Features.Cities.GetCities;

public class MobileGetCitiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/cities", async (IMediator mediator, Guid? governorateId, int? businessType) =>
        {
            var result = await mediator.Send(new GetCitiesQuery(governorateId, businessType));
            return Results.Ok(result);
        });
    }
}
