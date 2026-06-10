using Pulse.API.Features.Shared;
using MediatR;

namespace Pulse.API.Features.Laboratories.GetMobileLaboratories;

public class GetMobileLaboratoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/mobile/labs", async (HttpContext httpContext, IMediator mediator,
            Guid? governorateId, Guid? cityId, string? name,
            string? sortBy, string? sortDirection,
            int page = 1, int pageSize = 10) =>
        {
            var bq = new BusinessQuery(governorateId, cityId, name, sortBy, sortDirection, page, pageSize);
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var result = await mediator.Send(new GetMobileLaboratoriesQuery(bq, baseUrl));
            return Results.Ok(result);
        });
    }
}
