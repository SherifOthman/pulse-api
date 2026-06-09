using System.Security.Claims;
using MediatR;

namespace Pulse.API.Features.Laboratories.GetMobileLaboratoryDetails;

public class GetMobileLaboratoryDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/labs/{id:guid}", async (Guid id, HttpContext httpContext, IMediator mediator) =>
        {
            Guid? userId = null;
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdStr, out var parsed))
                    userId = parsed;
            }

            var result = await mediator.Send(new GetMobileLaboratoryDetailsQuery(id, userId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
