using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Cities.GetCities;

public class GetCitiesHandler(AppDbContext db)
    : IRequestHandler<GetCitiesQuery, List<CityResponse>>
{
    public async Task<List<CityResponse>> Handle(GetCitiesQuery request, CancellationToken ct)
    {
        var query = db.Set<City>().AsNoTracking();

        if (request.BusinessType.HasValue && Enum.IsDefined(typeof(BusinessType), request.BusinessType.Value))
        {
            var type = (BusinessType)request.BusinessType.Value;
            query = query.Where(c =>
                c.Businesses.Any(b => b.Type == type && b.ParentBusinessId == null));
        }

        if (request.GovernorateId.HasValue)
            query = query.Where(c => c.GovernorateId == request.GovernorateId.Value);

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CityResponse(c.Id, c.GovernorateId, c.Name))
            .ToListAsync(ct);
    }
}
