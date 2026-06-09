using Pulse.API.Domain.Enums;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Dashboard;

public class DashboardHandler(AppDbContext db) : IRequestHandler<DashboardQuery, DashboardResponse>
{
    public async Task<DashboardResponse> Handle(DashboardQuery request, CancellationToken ct)
    {
        var doctorsCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Doctor && b.ParentBusinessId == null, ct);

        var pharmaciesCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Pharmacy && b.ParentBusinessId == null, ct);

        var labsCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Laboratory && b.ParentBusinessId == null, ct);

        var radiologyCount = await db.Businesses
            .CountAsync(b => b.Type == BusinessType.Radiology && b.ParentBusinessId == null, ct);

        return new DashboardResponse(doctorsCount, pharmaciesCount, labsCount, radiologyCount);
    }
}
