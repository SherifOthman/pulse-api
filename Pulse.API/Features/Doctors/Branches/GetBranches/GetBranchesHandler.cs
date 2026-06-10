using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Branches.GetBranches;

public class GetBranchesHandler(AppDbContext db)
    : IRequestHandler<GetBranchesQuery, List<BranchListResponse>>
{
    public async Task<List<BranchListResponse>> Handle(GetBranchesQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.DayOfWeek;
        var now   = TimeOnly.FromDateTime(DateTime.UtcNow);

        var branches = await db.Businesses
            .AsNoTracking()
            .Where(b => b.ParentBusinessId == request.DoctorId && b.Type == BusinessType.Doctor)
            .Select(b => new
            {
                b.Id,
                b.Name,
                b.ProfileImageUrl,
                Governorate = b.City.Governorate.Name,
                City        = b.City.Name,
                VisitPrice  = b.Doctor!.VisitPrice,
                // Project working day data as a plain DTO list — avoids EF loading nav prop post-query
                WorkingDays = b.WorkingDays
                    .Select(w => new { w.Day, w.StartTime, w.EndTime })
                    .ToList(),
            })
            .ToListAsync(ct);

        return branches.Select(b =>
        {
            // Determine open status in memory after projection
            var todayRecord = b.WorkingDays.FirstOrDefault(w => w.Day == today);
            var isOpen = todayRecord is not null
                && todayRecord.StartTime <= now
                && todayRecord.EndTime   >= now;

            return new BranchListResponse(
                b.Id,
                b.Name,
                b.ProfileImageUrl,
                b.Governorate,
                b.City,
                b.VisitPrice,
                isOpen
            );
        }).ToList();
    }
}
