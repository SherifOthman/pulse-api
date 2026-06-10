using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.UpdateDoctor;

public class UpdateDoctorHandler(AppDbContext db)
    : IRequestHandler<UpdateDoctorCommand, UpdateDoctorResponse>
{
    public async Task<UpdateDoctorResponse> Handle(UpdateDoctorCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .Include(b => b.Doctor)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Doctor, ct);

        if (business is null)
            throw new NotFoundException("Doctor not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            business.Name = request.Name.Trim();

        if (request.CityId.HasValue)
        {
            if (!await db.Set<City>().AnyAsync(c => c.Id == request.CityId.Value, ct))
                throw new NotFoundException("City not found");
            business.CityId = request.CityId.Value;
        }

        if (request.Address is not null)
            business.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (request.Description is not null)
            business.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        if (request.ProfileImageUrl is not null)
            business.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();

        if (request.CoverImageUrl is not null)
            business.CoverImageUrl = string.IsNullOrWhiteSpace(request.CoverImageUrl) ? null : request.CoverImageUrl.Trim();

        if (business.Doctor is not null)
        {
            if (request.SpecializationId.HasValue)
            {
                if (!await db.Set<Specialization>().AnyAsync(s => s.Id == request.SpecializationId.Value, ct))
                    throw new NotFoundException("Specialization not found");
                business.Doctor.SpecializationId = request.SpecializationId.Value;
            }

            if (request.VisitPrice.HasValue)
                business.Doctor.VisitPrice = request.VisitPrice;

            if (request.Gender.HasValue)
                business.Doctor.Gender = request.Gender.Value;
        }

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

        if (request.Branches is not null)
        {
            var existingBranches = await db.Businesses
                .Where(b => b.ParentBusinessId == request.Id)
                .ToListAsync(ct);
            db.Businesses.RemoveRange(existingBranches);

            foreach (var br in request.Branches)
            {
                var branch = new Business
                {
                    Name = br.Name.Trim(),
                    Type = BusinessType.Doctor,
                    CityId = business.CityId,
                    Address = br.Address?.Trim(),
                    ParentBusinessId = request.Id,
                    CreatedByUserId = business.CreatedByUserId,
                };
                db.Businesses.Add(branch);

                if (br.WorkingDays is not null)
                {
                    foreach (var wd in br.WorkingDays)
                    {
                        db.Set<WorkingDay>().Add(new WorkingDay
                        {
                            BusinessId = branch.Id,
                            Day = (System.DayOfWeek)wd.Day,
                            StartTime = TimeOnly.Parse(wd.StartTime),
                            EndTime = TimeOnly.Parse(wd.EndTime),
                        });
                    }
                }

                if (br.PhoneNumbers is not null)
                {
                    foreach (var pn in br.PhoneNumbers)
                    {
                        db.Set<PhoneNumber>().Add(new PhoneNumber
                        {
                            BusinessId = branch.Id,
                            Number = pn.Number,
                            Type = pn.Type,
                        });
                    }
                }
            }
        }

        await db.SaveChangesAsync(ct);
        return new UpdateDoctorResponse(business.Id, business.Name);
    }
}
