using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Specializations.GetSpecializations;

public class GetSpecializationsHandler(AppDbContext db)
    : IRequestHandler<GetSpecializationsQuery, List<SpecializationResponse>>
{
    public async Task<List<SpecializationResponse>> Handle(GetSpecializationsQuery request, CancellationToken ct)
    {
        return await db.Set<Specialization>()
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SpecializationResponse(s.Id, s.Name))
            .ToListAsync(ct);
    }
}
