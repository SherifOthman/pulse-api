using MediatR;

namespace Pulse.API.Features.Cities.DeleteCity;

public class DeleteCityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/cities/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteCityCommand(id));
            return Results.NoContent();
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
