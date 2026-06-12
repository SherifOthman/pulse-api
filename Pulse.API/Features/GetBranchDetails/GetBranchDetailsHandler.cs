using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Branches.GetBranchDetails;

public class GetBranchDetailsHandler(AppDbContext db)
    : IRequestHandler<GetBranchDetailsQuery, BranchDetailsResponse?>
{
    public async Task<BranchDetailsResponse?> Handle(GetBranchDetailsQuery request, CancellationToken ct)
    {
        var b = await db.Branches
            .AsNoTracking()
            .Where(x => x.Id == request.Id && x.ParentBusiness.Type == BusinessType.Doctor)
            .Select(x => new
            {
                x.Id, x.Name, x.Address,
                x.Latitude, x.Longitude,
                CityId          = x.City.Id,
                GovernorateId   = x.City.Governorate.Id,
                GovernorateName = x.City.Governorate.Name,
                CityName        = x.City.Name,
                VisitPrice      = x.DoctorBranchProfile != null ? x.DoctorBranchProfile.VisitPrice : null,
                WorkingDays     = x.WorkingDays.Select(w => new { w.Day, w.StartTime, w.EndTime }).ToList(),
                PhoneNumbers    = x.PhoneNumbers.Select(p => new { p.Number, p.Type }).ToList(),
            })
            .FirstOrDefaultAsync(ct);

        if (b is null) return null;

        return new BranchDetailsResponse(
            b.Id, b.Name, b.Address,
            b.GovernorateName, b.GovernorateId, b.CityName, b.CityId,
            b.VisitPrice, b.Latitude, b.Longitude,
            b.WorkingDays.Select(w => new WorkingDayDto((int)w.Day, w.StartTime.ToString("HH:mm"), w.EndTime.ToString("HH:mm"))).OrderBy(w => w.Day).ToList(),
            b.PhoneNumbers.Select(p => new PhoneNumberDto(p.Number, p.Type)).ToList()
        );
    }
}
