using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Shared.Branches.GetBranches;

public class SharedGetBranchesHandler(AppDbContext db)
    : IRequestHandler<SharedGetBranchesQuery, List<BranchListResponse>>
{
    public async Task<List<BranchListResponse>> Handle(SharedGetBranchesQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.DayOfWeek;
        var now   = TimeOnly.FromDateTime(DateTime.UtcNow);

        var branches = await db.Businesses
            .AsNoTracking()
            .Where(b => b.ParentBusinessId == request.BusinessId && b.Type == request.Type)
            .Select(b => new
            {
                b.Id, b.Name, b.ProfileImageUrl,
                Governorate = b.City.Governorate.Name,
                City        = b.City.Name,
                WorkingDays = b.WorkingDays
                    .Select(w => new { w.Day, w.StartTime, w.EndTime })
                    .ToList(),
            })
            .ToListAsync(ct);

        return branches.Select(b =>
        {
            var todayRecord = b.WorkingDays.FirstOrDefault(w => w.Day == today);
            var isOpen = todayRecord is not null
                && todayRecord.StartTime <= now
                && todayRecord.EndTime   >= now;

            return new BranchListResponse(
                b.Id, b.Name, b.ProfileImageUrl,
                b.Governorate, b.City, isOpen
            );
        }).ToList();
    }
}
