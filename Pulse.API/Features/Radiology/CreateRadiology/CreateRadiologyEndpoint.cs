using MediatR;

namespace Pulse.API.Features.Radiology.CreateRadiology;

public class CreateRadiologyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/radiology", async (CreateRadiologyCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/dashboard/radiology/{result.Id}", result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
