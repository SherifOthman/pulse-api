using MediatR;

namespace Pulse.API.Features.Laboratories.CreateLaboratory;

public class CreateLaboratoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/labs", async (CreateLaboratoryCommand command, IMediator mediator) =>
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
