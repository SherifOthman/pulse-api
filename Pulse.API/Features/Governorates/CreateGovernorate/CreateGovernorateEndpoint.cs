using MediatR;

namespace Pulse.API.Features.Governorates.CreateGovernorate;

public class CreateGovernorateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/governorates", async (CreateGovernorateCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        });
    }
}
