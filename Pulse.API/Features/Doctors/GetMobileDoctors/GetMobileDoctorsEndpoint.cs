using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Doctors.GetMobileDoctors;

public class GetMobileDoctorsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/doctors", async (HttpContext httpContext, IMediator mediator,
            Guid? governorateId,
            Guid? cityId,
            string? name,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 10,
            int? gender = null,
            Guid? specializationId = null) =>
        {
            var bq = new BusinessQuery(governorateId, cityId, name, sortBy, sortDirection, page, pageSize);
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var result = await mediator.Send(new GetMobileDoctorsQuery(bq, (Gender?)gender, specializationId, baseUrl));
            return Results.Ok(result);
        });
    }
}
