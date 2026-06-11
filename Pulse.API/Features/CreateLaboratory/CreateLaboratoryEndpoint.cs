using MediatR;

namespace Pulse.API.Features.Laboratories.CreateLaboratory;

public class CreateLaboratoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/labs", async (CreateLaboratoryCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/dashboard/labs/{result.Id}", result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
