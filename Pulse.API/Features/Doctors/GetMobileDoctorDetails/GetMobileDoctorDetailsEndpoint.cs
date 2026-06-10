using System.Security.Claims;
using MediatR;

namespace Pulse.API.Features.Doctors.GetMobileDoctorDetails;

public class GetMobileDoctorDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/doctors/{id:guid}", async (Guid id, HttpContext httpContext, IMediator mediator) =>
        {
            Guid? userId = null;
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdStr, out var parsed))
                    userId = parsed;
            }

            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var result = await mediator.Send(new GetMobileDoctorDetailsQuery(id, userId, baseUrl));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
