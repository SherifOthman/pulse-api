using MediatR;

namespace Pulse.API.Features.Cities.UpdateCity;

public class UpdateCityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/cities/{id:guid}", async (Guid id, UpdateCityCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { Id = id });
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
