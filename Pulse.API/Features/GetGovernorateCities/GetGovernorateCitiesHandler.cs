using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using Pulse.API.Features.Cities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Governorates.GetGovernorateCities;

public class GetGovernorateCitiesHandler(AppDbContext db)
    : IRequestHandler<GetGovernorateCitiesQuery, List<CityResponse>>
{
    public async Task<List<CityResponse>> Handle(GetGovernorateCitiesQuery request, CancellationToken ct)
    {
        var exists = await db.Set<Governorate>().AnyAsync(g => g.Id == request.Id, ct);
        if (!exists)
            throw new KeyNotFoundException("Governorate not found");

        return await db.Set<City>()
            .AsNoTracking()
            .Where(c => c.GovernorateId == request.Id)
            .OrderBy(c => c.Name)
            .Select(c => new CityResponse(c.Id, c.GovernorateId, c.Name))
            .ToListAsync(ct);
    }
}
