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
                var s = httpContext.User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (Guid.TryParse(s, out var parsed)) userId = parsed;
            }
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var result = await mediator.Send(new GetMobileRadiologyDetailsQuery(id, userId, baseUrl));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
