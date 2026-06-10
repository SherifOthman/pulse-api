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
            .Include(b => b.WorkingDays)
            .Include(b => b.PhoneNumbers)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Doctor && b.ParentBusinessId != null, ct);

        if (branch is null)
            throw new NotFoundException("Branch not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            branch.Name = request.Name.Trim();

        if (request.CityId.HasValue)
            branch.CityId = request.CityId.Value;

        if (request.Address is not null)
            branch.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (request.Latitude.HasValue)
            branch.Latitude = request.Latitude;

        if (request.Longitude.HasValue)
            branch.Longitude = request.Longitude;

        if (branch.Doctor is not null && request.VisitPrice.HasValue)
            branch.Doctor.VisitPrice = request.VisitPrice;

        if (request.WorkingDays is not null)
        {
            foreach (var wd in request.WorkingDays)
            {
                if (!TimeOnly.TryParse(wd.StartTime, out var start) || !TimeOnly.TryParse(wd.EndTime, out var end))
                    throw new BadRequestException($"Invalid time format for day {wd.Day}");
                if (start >= end)
                    throw new BadRequestException($"Start time must be before end time for day {wd.Day}");
            }

            branch.WorkingDays = request.WorkingDays.Select(wd => new WorkingDay
            {
                Day = (System.DayOfWeek)wd.Day,
                StartTime = TimeOnly.Parse(wd.StartTime),
                EndTime = TimeOnly.Parse(wd.EndTime),
            }).ToList();
        }

        if (request.PhoneNumbers is not null)
        {
            branch.PhoneNumbers = request.PhoneNumbers.Select(pn => new PhoneNumber
            {
                Number = pn.Number,
                Type = pn.Type,
            }).ToList();
        }

        await db.SaveChangesAsync(ct);
        return new UpdateBranchResponse(branch.Id, branch.Name);
    }
}
