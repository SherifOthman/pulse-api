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
            .Include(b => b.WorkingDays)
            .Include(b => b.PhoneNumbers)
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
            foreach (var wd in request.WorkingDays)
            {
                if (!TimeOnly.TryParse(wd.StartTime, out var start) || !TimeOnly.TryParse(wd.EndTime, out var end))
                    throw new BadRequestException($"Invalid time format for day {wd.Day}");
                if (start >= end)
                    throw new BadRequestException($"Start time must be before end time for day {wd.Day}");
            }

            business.WorkingDays = request.WorkingDays.Select(wd => new WorkingDay
            {
                Day = (System.DayOfWeek)wd.Day,
                StartTime = TimeOnly.Parse(wd.StartTime),
                EndTime = TimeOnly.Parse(wd.EndTime),
            }).ToList();
        }

        if (request.PhoneNumbers is not null)
        {
            business.PhoneNumbers = request.PhoneNumbers.Select(pn => new PhoneNumber
            {
                Number = pn.Number,
                Type = pn.Type,
            }).ToList();
        }

        await db.SaveChangesAsync(ct);
        return new UpdateDoctorResponse(business.Id, business.Name);
    }
}
