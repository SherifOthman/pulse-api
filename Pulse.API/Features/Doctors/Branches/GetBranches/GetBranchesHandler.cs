using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
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
        var now = TimeOnly.FromDateTime(DateTime.UtcNow);

        var branches = await db.Businesses
            .AsNoTracking()
            .Where(b => b.ParentBusinessId == request.DoctorId && b.Type == BusinessType.Doctor)
            .Select(b => new
            {
                b.Id,
                b.Name,
                b.ProfileImageUrl,
                Governorate = b.City.Governorate.Name,
                City = b.City.Name,
                b.Doctor!.VisitPrice,
                b.WorkingDays,
            })
            .ToListAsync(ct);

        return branches.Select(b =>
        {
            var schedule = DoctorScheduleHelper.Compute(b.WorkingDays, today, now);
            return new BranchListResponse(
                b.Id, b.Name, b.ProfileImageUrl,
                b.Governorate, b.City, b.VisitPrice,
                schedule.IsOpen
            );
        }).ToList();
    }
}
