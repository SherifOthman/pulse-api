using MediatR;

namespace Pulse.API.Features.Cities.GetCities;

public class GetCitiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/cities", async (IMediator mediator, Guid? governorateId, int? businessType) =>
        {
            var result = await mediator.Send(new GetCitiesQuery(governorateId, businessType));
            return Results.Ok(result);
        });
    }
}
