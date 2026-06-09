using System.Security.Claims;
using MediatR;

namespace Pulse.API.Features.Radiology.GetMobileRadiologyDetails;

public class GetMobileRadiologyDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/radiology/{id:guid}", async (Guid id, HttpContext httpContext, IMediator mediator) =>
        {
            Guid? userId = null;
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdStr, out var parsed))
                    userId = parsed;
            }

            var result = await mediator.Send(new GetMobileRadiologyDetailsQuery(id, userId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
