using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Doctors.GetDoctors;

public class GetDoctorsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/doctors", async (IMediator mediator,
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
            var result = await mediator.Send(new GetDoctorsQuery(bq, (Gender?)gender, specializationId));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
