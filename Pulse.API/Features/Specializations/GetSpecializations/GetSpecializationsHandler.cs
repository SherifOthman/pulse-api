using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Specializations.GetSpecializations;

public class GetSpecializationsHandler(AppDbContext db)
    : IRequestHandler<GetSpecializationsQuery, List<SpecializationResponse>>
{
    public async Task<List<SpecializationResponse>> Handle(GetSpecializationsQuery request, CancellationToken ct)
    {
        var query = db.Set<Specialization>().AsNoTracking();

        if (request.BusinessType.HasValue && Enum.IsDefined(typeof(BusinessType), request.BusinessType.Value))
        {
            var type = (BusinessType)request.BusinessType.Value;
            query = query.Where(s => s.Doctors.Any(d => d.Business.Type == type && d.Business.ParentBusinessId == null));
        }

        return await query
            .OrderBy(s => s.Name)
            .Select(s => new SpecializationResponse(s.Id, s.Name))
            .ToListAsync(ct);
    }
}
