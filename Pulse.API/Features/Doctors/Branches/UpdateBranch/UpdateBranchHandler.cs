using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Branches.UpdateBranch;

public class UpdateBranchHandler(AppDbContext db)
    : IRequestHandler<UpdateBranchCommand, UpdateBranchResponse>
{
    public async Task<UpdateBranchResponse> Handle(UpdateBranchCommand request, CancellationToken ct)
    {
        var branch = await db.Businesses
            .Include(b => b.Doctor)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Doctor && b.ParentBusinessId != null, ct);

        if (branch is null)
            throw new NotFoundException("Branch not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            branch.Name = request.Name.Trim();

        if (request.CityId.HasValue)
            branch.CityId = request.CityId.Value;

        if (request.Address is not null)
            branch.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (request.Description is not null)
            branch.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        if (request.ProfileImageUrl is not null)
            branch.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();

        if (request.CoverImageUrl is not null)
            branch.CoverImageUrl = string.IsNullOrWhiteSpace(request.CoverImageUrl) ? null : request.CoverImageUrl.Trim();

        if (request.Latitude.HasValue)
            branch.Latitude = request.Latitude;

        if (request.Longitude.HasValue)
            branch.Longitude = request.Longitude;

        if (branch.Doctor is not null && request.VisitPrice.HasValue)
            branch.Doctor.VisitPrice = request.VisitPrice;

        if (request.WorkingDays is not null)
        {
            var existing = await db.Set<WorkingDay>().Where(w => w.BusinessId == request.Id).ToListAsync(ct);
            db.Set<WorkingDay>().RemoveRange(existing);

            foreach (var wd in request.WorkingDays)
            {
                db.Set<WorkingDay>().Add(new WorkingDay
                {
                    BusinessId = request.Id,
                    Day = (System.DayOfWeek)wd.Day,
                    StartTime = TimeOnly.Parse(wd.StartTime),
                    EndTime = TimeOnly.Parse(wd.EndTime),
                });
            }
        }

        if (request.PhoneNumbers is not null)
        {
            var existing = await db.Set<PhoneNumber>().Where(p => p.BusinessId == request.Id).ToListAsync(ct);
            db.Set<PhoneNumber>().RemoveRange(existing);

            foreach (var pn in request.PhoneNumbers)
            {
                db.Set<PhoneNumber>().Add(new PhoneNumber
                {
                    BusinessId = request.Id,
                    Number = pn.Number,
                    Type = pn.Type,
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return new UpdateBranchResponse(branch.Id, branch.Name);
    }
}
