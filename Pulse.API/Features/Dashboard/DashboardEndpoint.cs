using Pulse.API.Features;
using MediatR;

namespace Pulse.API.Features.Dashboard;

public class DashboardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/stats", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new DashboardQuery());
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
