using MediatR;

namespace Pulse.API.Features.Cities.CreateCity;

public class CreateCityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/cities", async (CreateCityCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        });
    }
}
