using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Governorates.GetGovernorates;

public class GetGovernoratesHandler(AppDbContext db)
    : IRequestHandler<GetGovernoratesQuery, List<GovernorateResponse>>
{
    public async Task<List<GovernorateResponse>> Handle(GetGovernoratesQuery request, CancellationToken ct)
    {
        var query = db.Set<Governorate>().AsNoTracking();

        if (request.BusinessType.HasValue && Enum.IsDefined(typeof(BusinessType), request.BusinessType.Value))
        {
            var type = (BusinessType)request.BusinessType.Value;
            query = query.Where(g => g.Cities.Any(c =>
                c.Businesses.Any(b => b.Type == type && b.ParentBusinessId == null)));
        }
        else
        {
            query = query.Where(g => g.Cities.Any(c => c.Businesses.Any()));
        }

        return await query
            .OrderBy(g => g.Name)
            .Select(g => new GovernorateResponse(g.Id, g.Name))
            .ToListAsync(ct);
    }
}
