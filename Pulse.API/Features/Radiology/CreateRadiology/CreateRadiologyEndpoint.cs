using MediatR;

namespace Pulse.API.Features.Radiology.CreateRadiology;

public class CreateRadiologyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/radiology", async (CreateRadiologyCommand command, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
